using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Aggregates.WishlistAggregate;
using Shopping.Infrastructure.DbContexts;

namespace Shopping.Infrastructure.EntityConfigurations;

internal sealed class WishlistItemEntityTypeConfiguration
    : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder.ToTable(nameof(WishlistItem), ShoppingDbContextMaster.DefaultSchema);

        builder.HasKey(wi => new { wi.WishlistId, wi.ProductId });

        builder.Property(item => item.ProductId).IsRequired();
        builder.Property(item => item.ProductName).IsRequired();
        builder.Property(item => item.ProductPrice).IsRequired();
    }
}
