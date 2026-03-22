using Core.Domain.Aggregates.IdentityAggregate.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Presentation.ServiceCollectionExtensions;

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
