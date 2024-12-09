namespace Pokedex.Infrastructure.Services.Translators;

public interface ITranslateService
{
    Task<string?> TranslateAsync(string text, TranslateLanguage language);
}