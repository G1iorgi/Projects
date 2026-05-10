using Core.Domain.Aggregates.IdentityAggregate.Configurations;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Configurations;

namespace Core.Presentation.ServiceCollectionExtensions;

internal static class OptionsServiceCollectionExtensions
{
    internal static void AddCustomOptions(this IServiceCollection services)
    {
        services.AddTokenValidationOptions();
        services.AddMessageBrokerOptions();
    }

    private static void AddTokenValidationOptions(this IServiceCollection services)
        => services
            .AddOptions<TokenValidationOptions>()
            .BindConfiguration(TokenValidationOptions.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();

    private static void AddMessageBrokerOptions(this IServiceCollection services)
        => services
            .AddOptions<MessageBrokerOptions>()
            .BindConfiguration(MessageBrokerOptions.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();
}
