using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Infrastructure.ApiProviders.CartApiProvider;
using Payment.Infrastructure.ApiProviders.DigitalBankApiProvider;
using Payment.Infrastructure.ApiProviders.OrderApiProvider;
using Payment.Infrastructure.ApiProviders.ProductApiProvider;

namespace Payment.Infrastructure.ApiProviders;

internal static class ApiProvidersServiceCollectionExtensions
{
    internal static void AddApiProviders(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDigitalBankApiProvider(configuration);
        services.AddCartApiProvider(configuration);
        services.AddProductApiProvider(configuration);
        services.AddOrderApiProvider(configuration);
    }
}
