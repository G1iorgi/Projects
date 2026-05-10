using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Infrastructure.ApiProviders;
using Shopping.Infrastructure.Dapper;
using Shopping.Infrastructure.DbContexts;
using Shopping.Infrastructure.Messaging;
using Shopping.Infrastructure.Repositories;

namespace Shopping.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Guard.Against.Null(services);
        Guard.Against.Null(configuration);

        services.AddCartDbContexts(configuration);
        services.AddSqlConnectionFactory(configuration);
        services.AddRepositories();
        services.AddMessaging(configuration);
        services.AddApiProviders(configuration);

        return services;
    }
}
