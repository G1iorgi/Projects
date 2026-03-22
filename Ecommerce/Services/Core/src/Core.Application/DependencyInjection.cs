using Ardalis.GuardClauses;
using Core.Application.Aggregates;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Guard.Against.Null(services);

        services.AddServices();

        return services;
    }
}
