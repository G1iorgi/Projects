using Ardalis.GuardClauses;
using Core.Domain.Aggregates.CartAggregate;
using Core.Domain.Aggregates.CategoryAggregate;
using Core.Domain.Aggregates.IdentityAggregate;
using Core.Domain.Aggregates.ProductAggregate;
using Core.Domain.Aggregates.WishlistAggregate;
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

    public DbSet<Wishlist> Wishlists { get; set; }

    public DbSet<Cart> Carts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        Guard.Against.Null(builder);

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ProductEntityTypeConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(CategoryEntityTypeConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(RefreshTokenEntityTypeConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(WishlistEntityTypeConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(CartEntityTypeConfiguration).Assembly);
    }
}
