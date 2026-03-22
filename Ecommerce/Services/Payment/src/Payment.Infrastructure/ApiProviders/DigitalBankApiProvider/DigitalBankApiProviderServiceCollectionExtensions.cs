using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Domain.Aggregates.PaymentAggregate.Configurations;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider;

namespace Payment.Infrastructure.ApiProviders.DigitalBankApiProvider;

internal static class DigitalBankApiProviderServiceCollectionExtensions
{
    internal static void AddDigitalBankApiProvider(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind options
        services.Configure<DigitalBankOptions>(
            configuration.GetSection(DigitalBankOptions.Key));

        // Validate options immediately
        var digitalBankOptions = configuration
            .GetSection(DigitalBankOptions.Key)
            .Get<DigitalBankOptions>();

        Guard.Against.Null(digitalBankOptions);
        Guard.Against.NullOrWhiteSpace(digitalBankOptions.BaseUrl);

        // Register HttpClient
        services.AddHttpClient<IPaymentApiProvider, DigitalBankApiProvider>(client =>
        {
            client.BaseAddress = new Uri(digitalBankOptions.BaseUrl);
        });
    }
}
