using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shopping.Infrastructure.DbContexts;

internal static class DbContextServiceCollectionExtensions
{
    internal static void AddCartDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShoppingDbContextMaster>(options =>
        {
            options
                .UseLazyLoadingProxies()
                .UseNpgsql(configuration.GetConnectionString(nameof(ShoppingDbContextMaster)),
                    npgsqlBuilder =>
                        npgsqlBuilder.MigrationsHistoryTable(ShoppingDbContextMaster.MigrationHistoryTable,
                            ShoppingDbContextMaster.DefaultSchema));
        });
    }
}
