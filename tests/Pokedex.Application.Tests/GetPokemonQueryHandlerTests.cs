using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PokeApiNet;
using Pokedex.Application.Pokemon.GetPokemon;
using Pokedex.Infrastructure.Services.PokeApi;
using Pokedex.Infrastructure.Services.Translators;
using Xunit;

namespace Pokedex.Application.Tests;

public class GetPokemonQueryHandlerTests
{
    private readonly Mock<IPokeApiService> _pokeApiServiceMock;
    private readonly Mock<ITranslateService> _translateServiceMock;
    private readonly Mock<ILogger<GetPokemonQueryHandler>> _loggerMock;
    private readonly GetPokemonQueryHandler _handler;

    public GetPokemonQueryHandlerTests()
    {
        _pokeApiServiceMock = new Mock<IPokeApiService>();
        _translateServiceMock = new Mock<ITranslateService>();
        _loggerMock = new Mock<ILogger<GetPokemonQueryHandler>>();
        _handler = new GetPokemonQueryHandler(_pokeApiServiceMock.Object, _translateServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPokemonResponse_WhenPokemonExists()
    {
        // Arrange
        var query = new GetPokemonQuery { Name = "pikachu", Translate = false };
        var species = new PokemonSpecies
        {
            FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>
            {
                new PokemonSpeciesFlavorTexts { Language = new NamedApiResource<Language> { Name = "en" }, FlavorText = "Electric mouse" }
            },
            Habitat = new NamedApiResource<PokemonHabitat> { Name = "forest" },
            IsLegendary = false
        };
        _pokeApiServiceMock.Setup(client => client.GetPokemonSpeciesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(species);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("pikachu", result.Name);
        Assert.Equal("Electric mouse", result.Description);
        Assert.Equal("forest", result.Habitat);
        Assert.False(result.IsLegendary);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenPokemonNotFound()
    {
        // Arrange
        var query = new GetPokemonQuery { Name = "unknown", Translate = false };
        _pokeApiServiceMock.Setup(client => client.GetPokemonSpeciesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException(null, null, HttpStatusCode.NotFound));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldTranslateToShakespeare_WhenTranslateIsTrue()
    {
        // Arrange
        var query = new GetPokemonQuery { Name = "pikachu", Translate = true };
        var species = new PokemonSpecies
        {
            FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>
            {
                new PokemonSpeciesFlavorTexts { Language = new NamedApiResource<Language> { Name = "en" }, FlavorText = "Electric mouse" }
            },
            Habitat = new NamedApiResource<PokemonHabitat> { Name = "forest" },
            IsLegendary = false
        };
        _pokeApiServiceMock.Setup(client => client.GetPokemonSpeciesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(species);
        _translateServiceMock.Setup(service => service.TranslateAsync(It.IsAny<string>(), TranslateLanguage.Shakespeare))
            .ReturnsAsync("Translated description");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Translated description", result.Description);
    }
    
    [Theory]
    [InlineData("cave", false)]
    [InlineData("forest", true)]
    public async Task Handle_ShouldTranslateToYodaWhenFromCaveOrLegendary(string habitat, bool isLegendary)
    {
        // Arrange
        var query = new GetPokemonQuery { Name = "pikachu", Translate = true };
        var species = new PokemonSpecies
        {
            FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>
            {
                new PokemonSpeciesFlavorTexts { Language = new NamedApiResource<Language> { Name = "en" }, FlavorText = "Electric mouse" }
            },
            Habitat = new NamedApiResource<PokemonHabitat> { Name = habitat },
            IsLegendary = isLegendary
        };
        _pokeApiServiceMock.Setup(client => client.GetPokemonSpeciesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(species);
        _translateServiceMock.Setup(service => service.TranslateAsync(It.IsAny<string>(), TranslateLanguage.Yoda))
            .ReturnsAsync("Translated to yoda");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Translated to yoda", result.Description);
    }
}