using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Pokedex.Infrastructure.Services.Translators;

public class FunTranslateService(HttpClient client, ILogger<FunTranslateService> logger) : ITranslateService
{
    public async Task<string?> TranslateAsync(string text, TranslateLanguage language)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;
        
        try
        {
            var uri = $"translate/{language}.json";
            var response = await client.PostAsJsonAsync(uri, new { text });
            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            var result = await response.Content.ReadFromJsonAsync<TranslationResponse>();
            return result?.Contents.Translated;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to translate text: {text}", text);
            return null;
        }
    }
}

public enum TranslateLanguage
{
    Shakespeare,
    Yoda
}

internal class TranslationResponse
{
    public required TranslationContents Contents { get; set; }
}

internal class TranslationContents
{
    public required string Translated { get; set; }
}