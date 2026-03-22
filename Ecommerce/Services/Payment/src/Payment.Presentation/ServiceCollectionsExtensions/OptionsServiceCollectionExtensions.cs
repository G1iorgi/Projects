using Microsoft.Extensions.DependencyInjection;
using Payment.Presentation.Configurations;

namespace Payment.Presentation.ServiceCollectionsExtensions;

internal static class OptionsServiceCollectionExtensions
{
    internal static void AddCustomOptions(this IServiceCollection services)
    {
        services.AddTokenValidationOptions();
    }

    private static void AddTokenValidationOptions(this IServiceCollection services)
        => services
            .AddOptions<TokenValidationOptions>()
            .BindConfiguration(TokenValidationOptions.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();
}
