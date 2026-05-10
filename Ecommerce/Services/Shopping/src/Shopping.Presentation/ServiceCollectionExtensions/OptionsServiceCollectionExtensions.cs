using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Configurations;
using Shopping.Domain.Aggregates.ProductAggregate.Configurations;
using Shopping.Presentation.Configurations;

namespace Shopping.Presentation.ServiceCollectionExtensions;

internal static class OptionsServiceCollectionExtensions
{
    internal static void AddCustomOptions(this IServiceCollection services)
    {
        services.AddTokenValidationOptions();
        services.AddProductOptions();
        services.AddMessageBrokerOptions();
    }

    private static void AddTokenValidationOptions(this IServiceCollection services)
        => services
            .AddOptions<TokenValidationOptions>()
            .BindConfiguration(TokenValidationOptions.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();

    private static void AddProductOptions(this IServiceCollection services)
        => services
            .AddOptions<ProductApiProviderOptions>()
            .BindConfiguration(ProductApiProviderOptions.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();

    private static void AddMessageBrokerOptions(this IServiceCollection services)
        => services
            .AddOptions<MessageBrokerOptions>()
            .BindConfiguration(MessageBrokerOptions.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();
}
