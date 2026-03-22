using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Payment.Domain.Aggregates.CartAggregate.CartApiProvider;
using Payment.Domain.Aggregates.CartAggregate.Configurations;

namespace Payment.Infrastructure.ApiProviders.CartApiProvider;

internal static class CartApiProviderServiceCollectionExtensions
{
    internal static void AddCartApiProvider(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind options
        services.Configure<CartApiProviderOptions>(
            configuration.GetSection(CartApiProviderOptions.Key));

        // Validate options immediately
        var digitalBankOptions = configuration
            .GetSection(CartApiProviderOptions.Key)
            .Get<CartApiProviderOptions>();

        Guard.Against.Null(digitalBankOptions);
        Guard.Against.NullOrWhiteSpace(digitalBankOptions.BaseUrl);

        // Register HttpClient
        services.AddHttpClient<ICartApiProvider, CartApiProvider>(client =>
        {
            client.BaseAddress = new Uri(digitalBankOptions.BaseUrl);
        });
    }
}
