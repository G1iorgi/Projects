using System.Reflection;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Aggregates.CartAggregate;
using Shopping.Domain.Aggregates.OrderAggregate;
using Shopping.Domain.Aggregates.WishlistAggregate;
using Shopping.Infrastructure.EntityConfigurations;

namespace Shopping.Infrastructure.DbContexts;

public class ShoppingDbContextMaster(DbContextOptions<ShoppingDbContextMaster> options)
    : DbContext(options)
{
    public const string MigrationHistoryTable = "__ShoppingMigrationsHistory";
    public const string DefaultSchema = "Shopping";

    public DbSet<Cart> Carts { get; set; }

    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<Wishlist> Wishlists { get; set; }

    public DbSet<WishlistItem> WishlistItems { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Guard.Against.Null(modelBuilder);
        base.OnModelCreating(modelBuilder);

        // TODO: Use the following line to automatically apply all configurations from the assembly
        // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartEntityTypeConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartItemEntityTypeConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WishlistEntityTypeConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WishlistItemEntityTypeConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderEntityTypeConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderItemEntityTypeConfiguration).Assembly);
    }
}
