using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.DbContexts;

internal static class DbContextServiceCollectionExtensions
{
    internal static void AddCoreDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CoreDbContextMaster>(
            options =>
            {
                options
                    .UseLazyLoadingProxies()
                    .UseNpgsql(
                        configuration.GetConnectionString(nameof(CoreDbContextMaster)),
                        npgsqlBuilder =>
                            npgsqlBuilder.MigrationsHistoryTable(
                                CoreDbContextMaster.MigrationHistoryTable,
                                CoreDbContextMaster.DefaultSchema));
            });

        // TODO get password requirements from configuration
        services.AddIdentity<IdentityUser, IdentityRole>(
                options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 9;
                })
            .AddEntityFrameworkStores<CoreDbContextMaster>()
            .AddDefaultTokenProviders();
    }
}
