using Ardalis.GuardClauses;
using Core.Domain.Aggregates.CategoryAggregate;
using Core.Domain.Aggregates.IdentityAggregate;
using Core.Domain.Aggregates.ProductAggregate;
using Core.Infrastructure.EntityConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.DbContexts;

public class CoreDbContextMaster(DbContextOptions<CoreDbContextMaster> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public const string MigrationHistoryTable = "__CoreMigrationsHistory";
    public const string DefaultSchema = "Core";

    public DbSet<Product> Products { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        Guard.Against.Null(builder);
        base.OnModelCreating(builder);

        // TODO: instead of one by one configuration, we can use reflection to apply all configurations in the assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ProductEntityTypeConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(CategoryEntityTypeConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(RefreshTokenEntityTypeConfiguration).Assembly);
    }
}
