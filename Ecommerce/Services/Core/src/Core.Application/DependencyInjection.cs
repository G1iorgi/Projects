using Ardalis.GuardClauses;
using Core.Application.Aggregates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        Guard.Against.Null(services);

        services.AddServices();

        return services;
    }
}
