using Core.Domain.Aggregates.IdentityAggregate.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Presentation.ServiceCollectionExtensions;

internal static class OptionsServiceCollectionExtensions
{
    public static IServiceCollection AddCustomOptions(this IServiceCollection services)
    {
        services
            .AddOptions<TokenValidationOptions>()
            .BindConfiguration(TokenValidationOptions.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
