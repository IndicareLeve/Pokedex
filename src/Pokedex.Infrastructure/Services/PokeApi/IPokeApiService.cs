using PokeApiNet;

namespace Pokedex.Infrastructure.Services.PokeApi;

public interface IPokeApiService
{
    Task<PokemonSpecies> GetPokemonSpeciesAsync(string name, CancellationToken cancellationToken);
}