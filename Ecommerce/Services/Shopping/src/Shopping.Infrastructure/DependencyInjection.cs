using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Domain.Aggregates.ProductAggregate.Configurations;
using Shopping.Infrastructure.ApiProviders;
using Shopping.Infrastructure.DbContexts;
using Shopping.Infrastructure.Repositories;

namespace Shopping.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Guard.Against.Null(services);
        Guard.Against.Null(configuration);

        services.AddCartDbContexts(configuration);
        services.AddRepositories();
        services.AddApiProviders(configuration);

        return services;
    }
}
