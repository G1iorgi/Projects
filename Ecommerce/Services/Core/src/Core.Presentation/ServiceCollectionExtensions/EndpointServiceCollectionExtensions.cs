using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Presentation.ServiceCollectionExtensions;

internal static class EndpointServiceCollectionExtensions
{
    public static IServiceCollection AddMinimalEndpoints(this IServiceCollection services)
    {
        var serviceDescriptors = typeof(DependencyInjection).Assembly
            .DefinedTypes
            .Where(
                type =>
                    type is { IsAbstract: false, IsInterface: false } &&
                    type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type));

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder UseMinimalEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapRoutes(app);
        }

        return app;
    }
}
