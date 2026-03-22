using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Domain.Aggregates.CartAggregate.Configurations;
using Payment.Domain.Aggregates.OrderAggregate.Configurations;
using Payment.Domain.Aggregates.PaymentAggregate.Configurations;
using Payment.Domain.Aggregates.ProductAggregate.Configurations;
using Payment.Infrastructure.ApiProviders;
using Payment.Infrastructure.ApiProviders.CartApiProvider;

namespace Payment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Guard.Against.Null(services);
        Guard.Against.Null(configuration);

        services.AddApiProviders(configuration);

        return services;
    }
}
