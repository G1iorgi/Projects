using Ardalis.GuardClauses;
using Core.Infrastructure.DbContexts;
using Core.Infrastructure.Repositories;
using Core.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Guard.Against.Null(services);
        Guard.Against.Null(configuration);

        services.AddCoreDbContexts(configuration);
        services.AddRepositories();
        services.AddServices();

        return services;
    }
}
