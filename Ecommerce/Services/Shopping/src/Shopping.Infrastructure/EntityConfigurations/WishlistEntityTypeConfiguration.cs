using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Aggregates.WishlistAggregate;
using Shopping.Infrastructure.DbContexts;

namespace Shopping.Infrastructure.EntityConfigurations;

internal sealed class WishlistEntityTypeConfiguration
    : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable(nameof(Wishlist), ShoppingDbContextMaster.DefaultSchema);

        builder.HasKey(wishlist => wishlist.Id);

        builder.Property(wishlist => wishlist.UserId).IsRequired();
    }
}
