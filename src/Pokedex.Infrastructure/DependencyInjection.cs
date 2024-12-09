using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pokedex.Infrastructure.Services.Translators;

namespace Pokedex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        AddFunTranslations(services, config);
        services.AddScoped<ITranslateService, FunTranslateService>();

        return services;
    }
    
    private static void AddFunTranslations(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<FunTranslateService>(client =>
        {
            var uri = config["FunTranslations:BaseUrl"];
            if (string.IsNullOrWhiteSpace(uri))
                throw new InvalidOperationException("FunTranslations:BaseUrl is missing from configuration");
            
            client.BaseAddress = new Uri(uri);
        });
    }
}