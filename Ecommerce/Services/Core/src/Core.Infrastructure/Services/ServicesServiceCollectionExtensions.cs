using Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Services;

internal static class ServicesServiceCollectionExtensions
{
    internal static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IMapperService, MapperServiceService>();
    }
}
