namespace Payment.Infrastructure.ApiProviders.ProductApiProvider;

using Ardalis.GuardClauses;
using Domain.Aggregates.ProductAggregate.Configurations;
using Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal static class ProductApiProviderServiceCollectionExtensions
{
    internal static void AddProductApiProvider(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind options
        services.Configure<ProductApiProviderOptions>(
            configuration.GetSection(ProductApiProviderOptions.Key));

        // Validate options immediately
        var productApiProvider = configuration
            .GetSection(ProductApiProviderOptions.Key)
            .Get<ProductApiProviderOptions>();

        Guard.Against.Null(productApiProvider);
        Guard.Against.NullOrWhiteSpace(productApiProvider.BaseUrl);

        // Register HttpClient
        services.AddHttpClient<IProductApiProvider, ProductApiProvider>(client =>
        {
            client.BaseAddress = new Uri(productApiProvider.BaseUrl);
        });
    }
}
