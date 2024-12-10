using MediatR;

namespace Pokedex.Application.Pokemon.GetPokemon;

public class GetPokemonQuery : IRequest<GetPokemonResponse>
{
    public required string Name { get; set; }
    public bool Translate { get; set; }
}

public class GetPokemonResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Habitat { get; set; }
    public bool IsLegendary { get; set; }
}