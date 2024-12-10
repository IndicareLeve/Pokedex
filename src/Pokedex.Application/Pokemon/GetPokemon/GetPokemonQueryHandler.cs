using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using PokeApiNet;
using Pokedex.Infrastructure.Services.Translators;

namespace Pokedex.Application.Pokemon.GetPokemon;

public class GetPokemonQueryHandler(
    PokeApiClient pokeApiClient,
    ITranslateService translateService,
    ILogger<GetPokemonQueryHandler> logger)
    : IRequestHandler<GetPokemonQuery, GetPokemonResponse?>
{
    public async Task<GetPokemonResponse?> Handle(GetPokemonQuery request, CancellationToken cancellationToken)
    {
        GetPokemonResponse? response = null;
        
        try
        {
            var pokemon = await pokeApiClient.GetResourceAsync<PokeApiNet.Pokemon>(request.Name, cancellationToken);
            var species = await pokeApiClient.GetResourceAsync<PokeApiNet.PokemonSpecies>(pokemon.Species.Name, cancellationToken);
            
            response = new GetPokemonResponse
            {
                Name = pokemon.Name,
                Description = species.FlavorTextEntries
                    .FirstOrDefault(f => f.Language.Name == "en")?.FlavorText
                    .Replace('\n', ' ')
                    .Replace('\t', ' ')
                    .Replace('\f', ' '),
                Habitat = species.Habitat?.Name,
                IsLegendary = species.IsLegendary
            };
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogWarning("Pokemon not found: {name}", request.Name);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get pokemon: {name}", request.Name);
            throw;
        }

        if (request.Translate)
            await TranslateDescriptionAsync(response);
        
        return response;
    }
    
    private async Task TranslateDescriptionAsync(GetPokemonResponse response)
    {
        if (string.IsNullOrWhiteSpace(response.Description))
            return;
        
        var language = response.IsLegendary || response.Habitat == "cave" ? TranslateLanguage.Yoda : TranslateLanguage.Shakespeare;
        var translation = await translateService.TranslateAsync(response.Description, language);
        if (translation is not null)
            response.Description = translation;
    }
}