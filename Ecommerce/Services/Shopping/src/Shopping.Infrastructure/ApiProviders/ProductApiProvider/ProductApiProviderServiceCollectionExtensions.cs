using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shopping.Domain.Aggregates.ProductAggregate.Configurations;
using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider;

namespace Shopping.Infrastructure.ApiProviders.ProductApiProvider;

internal static class ProductApiProviderServiceCollectionExtensions
{
    internal static void AddProductApiProvider(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind options
        services.Configure<ProductApiProviderOptions>(
            configuration.GetSection(ProductApiProviderOptions.Key));

        // Validate options immediately
        var digitalBankOptions = configuration
            .GetSection(ProductApiProviderOptions.Key)
            .Get<ProductApiProviderOptions>();

        Guard.Against.Null(digitalBankOptions);
        Guard.Against.NullOrWhiteSpace(digitalBankOptions.BaseUrl);

        // Register HttpClient
        services.AddHttpClient<IProductApiProvider, ProductApiProvider>(client =>
        {
            client.BaseAddress = new Uri(digitalBankOptions.BaseUrl);
        });
    }
}
