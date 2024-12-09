using Microsoft.AspNetCore.Http.HttpResults;
using Pokedex.API.Models;

namespace Pokedex.API.Apis;

public static class PokedexApi
{
    public static IEndpointRouteBuilder MapPokedexApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("pokemon/{name}", GetPokemon);
        app.MapGet("pokemon/translated/{name}", GetTranslatedPokemon);
        
        return app;
    }
    
    private static async Task<Results<Ok<Pokemon>, NotFound, BadRequest<string>>> GetPokemon(string name)
    {
        throw new NotImplementedException();
    }
    
    private static async Task<Results<Ok<Pokemon>, NotFound, BadRequest<string>>> GetTranslatedPokemon(string name)
    {
        throw new NotImplementedException();
    }
}