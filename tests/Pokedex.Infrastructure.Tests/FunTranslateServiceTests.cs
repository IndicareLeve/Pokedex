using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Pokedex.Infrastructure.Services.Translators;

namespace Pokedex.Infrastructure.Tests;

public class FunTranslateServiceTests
{
    private static readonly Uri FunTranslationsBaseAddress = new("https://api.funtranslations.com/");

    [Fact]
    public async Task? TranslateAsync_CorrectlyBuildsRequestBody()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        var request = default(HttpRequestMessage);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((r, _) => request = r)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"contents\": {\"translated\": \"translated text\"}}")
            });
        
        var client = new HttpClient(handler.Object);
        client.BaseAddress = FunTranslationsBaseAddress;
        var logger = new Mock<ILogger<FunTranslateService>>();
        var service = new FunTranslateService(client, logger.Object);

        // Act
        await service.TranslateAsync("some test string", TranslateLanguage.Shakespeare);

        // Assert
        Assert.NotNull(request);
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal($"{FunTranslationsBaseAddress}translate/Shakespeare.json", request.RequestUri?.ToString());
        Assert.Equal("{\"text\":\"some test string\"}", await request.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task TranslateAsync_WithValidResponse_ReturnsTranslatedText()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"contents\": {\"translated\": \"translated text\"}}")
            });
        
        var client = new HttpClient(handler.Object);
        client.BaseAddress = FunTranslationsBaseAddress;
        var logger = new Mock<ILogger<FunTranslateService>>();
        var service = new FunTranslateService(client, logger.Object);

        // Act
        var result = await service.TranslateAsync("some test string", TranslateLanguage.Shakespeare);

        // Assert
        Assert.Equal("translated text", result);
    }
    
    [Fact]
    public async Task TranslateAsync_WithEmptyText_ReturnsNull()
    {
        // Arrange
        var client = new HttpClient();
        client.BaseAddress = FunTranslationsBaseAddress;
        var logger = new Mock<ILogger<FunTranslateService>>();
        var service = new FunTranslateService(client, logger.Object);

        // Act
        var result = await service.TranslateAsync("", TranslateLanguage.Shakespeare);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task TranslateAsync_WithSuccessCodeAndEmptyResponse_ReturnsNull()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });
        
        var client = new HttpClient(handler.Object);
        client.BaseAddress = FunTranslationsBaseAddress;
        var logger = new Mock<ILogger<FunTranslateService>>();
        var service = new FunTranslateService(client, logger.Object);

        // Act
        var result = await service.TranslateAsync("text", TranslateLanguage.Shakespeare);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TranslateAsync_WithInvalidResponse_ReturnsNull()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });
        
        var client = new HttpClient(handler.Object);
        client.BaseAddress = FunTranslationsBaseAddress;
        var logger = new Mock<ILogger<FunTranslateService>>();
        var service = new FunTranslateService(client, logger.Object);

        // Act
        var result = await service.TranslateAsync("text", TranslateLanguage.Shakespeare);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TranslateAsync_WithException_ReturnsNull()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Throws(new Exception());
        
        var client = new HttpClient(handler.Object);
        client.BaseAddress = FunTranslationsBaseAddress;
        var logger = new Mock<ILogger<FunTranslateService>>();
        var service = new FunTranslateService(client, logger.Object);

        // Act
        var result = await service.TranslateAsync("text", TranslateLanguage.Shakespeare);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task TranslateAsync_UsesProperEndpointFromEnum()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        var request = default(HttpRequestMessage);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((r, _) => request = r)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"contents\": {\"translated\": \"translated text\"}}")
            });
        
        var client = new HttpClient(handler.Object);
        client.BaseAddress = FunTranslationsBaseAddress;
        var logger = new Mock<ILogger<FunTranslateService>>();
        var service = new FunTranslateService(client, logger.Object);

        // Act
        await service.TranslateAsync("text", TranslateLanguage.Yoda);

        // Assert
        Assert.Equal($"{FunTranslationsBaseAddress}translate/Yoda.json", request.RequestUri?.ToString());
    }
}