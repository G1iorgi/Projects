using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Payment.Domain.Aggregates.OrderAggregate.Configurations;
using Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider;

namespace Payment.Infrastructure.ApiProviders.OrderApiProvider;

internal static class OrderApiProviderServiceCollectionExtensions
{
    internal static void AddOrderApiProvider(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind options
        services.Configure<OrderApiProviderOptions>(
            configuration.GetSection(OrderApiProviderOptions.Key));

        // Validate options immediately
        var digitalBankOptions = configuration
            .GetSection(OrderApiProviderOptions.Key)
            .Get<OrderApiProviderOptions>();

        Guard.Against.Null(digitalBankOptions);
        Guard.Against.NullOrWhiteSpace(digitalBankOptions.BaseUrl);

        // Register HttpClient
        services.AddHttpClient<IOrderApiProvider, OrderApiProvider>(client =>
        {
            client.BaseAddress = new Uri(digitalBankOptions.BaseUrl);
        });
    }
}
