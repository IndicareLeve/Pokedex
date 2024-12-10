using PokeApiNet;

namespace Pokedex.Infrastructure.Services.PokeApi;

public class PokeApiService(PokeApiClient client) : IPokeApiService
{
    public Task<PokemonSpecies> GetPokemonSpeciesAsync(string name, CancellationToken cancellationToken)
    {
        return client.GetResourceAsync<PokemonSpecies>(name, cancellationToken);
    }
}