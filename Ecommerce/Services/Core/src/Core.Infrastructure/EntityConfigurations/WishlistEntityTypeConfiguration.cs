using Core.Domain.Aggregates.ProductAggregate;
using Core.Domain.Aggregates.WishlistAggregate;
using Core.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.EntityConfigurations;

internal sealed class WishlistEntityTypeConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable(nameof(Wishlist), CoreDbContextMaster.DefaultSchema);

        builder.HasKey(wishlist => wishlist.Id);

        builder.Property(wishlist => wishlist.UserId).IsRequired();
        builder.Property(wishlist => wishlist.ProductId).IsRequired();

        builder.HasIndex(wishlist => new { wishlist.UserId, wishlist.ProductId }).IsUnique();

        builder.HasOne(wishlist => wishlist.User)
            .WithMany()
            .HasForeignKey(wishlist => wishlist.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wishlist => wishlist.Product)
            .WithMany(product => product.Wishlists)
            .HasForeignKey(wishlist => wishlist.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(wishlist => wishlist.Product.Status != ProductStatus.Disabled);
    }
}
