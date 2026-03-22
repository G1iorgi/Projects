using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Infrastructure.ApiProviders.ProductApiProvider;

namespace Shopping.Infrastructure.ApiProviders;

internal static class ApiProvidersServiceCollectionExtensions
{
    internal static void AddApiProviders(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProductApiProvider(configuration);
    }
}
