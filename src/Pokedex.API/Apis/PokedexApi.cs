using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Pokedex.Application.Pokemon.GetPokemon;

namespace Pokedex.API.Apis;

public static class PokedexApi
{
    public static IEndpointRouteBuilder MapPokedexApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("pokemon/{name}", GetPokemon);
        app.MapGet("pokemon/translated/{name}", GetTranslatedPokemon);
        
        return app;
    }
    
    private static async Task<Results<Ok<GetPokemonResponse>, NotFound>> GetPokemon(
        IMediator mediator,
        string name)
    {
        var pokemon = await mediator.Send(new GetPokemonQuery { Name = name, Translate = false });
        return pokemon is null ? TypedResults.NotFound() : TypedResults.Ok(pokemon);
    }
    
    private static async Task<Results<Ok<GetPokemonResponse>, NotFound>> GetTranslatedPokemon(
        IMediator mediator,
        string name)
    {
        var pokemon = await mediator.Send(new GetPokemonQuery { Name = name, Translate = true });
        return pokemon is null ? TypedResults.NotFound() : TypedResults.Ok(pokemon);
    }
}