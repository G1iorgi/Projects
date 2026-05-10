using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Configurations;
using SharedKernel.Contracts.Abstractions.Data;

namespace Shopping.Infrastructure.Dapper;

internal static class SqlConnectionFactoryServiceCollectionExtensions
{
    internal static IServiceCollection AddSqlConnectionFactory(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConnectionStringsOptions>(
            configuration.GetSection(ConnectionStringsOptions.Key));

        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

        return services;
    }
}
