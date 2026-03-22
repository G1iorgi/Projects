using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Aggregates.CartAggregate;
using Shopping.Infrastructure.DbContexts;

namespace Shopping.Infrastructure.EntityConfigurations;

internal sealed class CartItemEntityTypeConfiguration
    : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable(nameof(CartItem), ShoppingDbContextMaster.DefaultSchema);

        builder.HasKey(ci => new { ci.CartId, ci.ProductId });

        builder.Property(item => item.ProductId).IsRequired();
        builder.Property(item => item.ProductName).IsRequired();
        builder.Property(item => item.ProductQuantity).IsRequired();
        builder.Property(item => item.ProductPrice).IsRequired();
    }
}
