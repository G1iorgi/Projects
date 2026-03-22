using Microsoft.Extensions.DependencyInjection;
using Shopping.Domain.Aggregates.ProductAggregate.Configurations;
using Shopping.Presentation.Configurations;

namespace Shopping.Presentation.ServiceCollectionExtensions;

internal static class OptionsServiceCollectionExtensions
{
    internal static void AddCustomOptions(this IServiceCollection services)
    {
        services.AddTokenValidationOptions();
        services.AddProductOptions();
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
}
