using Core.Domain.Aggregates.CartAggregate;
using Core.Domain.Aggregates.ProductAggregate;
using Core.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.EntityConfigurations;

internal sealed class CartEntityTypeConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable(nameof(Cart), CoreDbContextMaster.DefaultSchema);

        builder.HasKey(cart => cart.Id);

        builder.Property(cart => cart.UserId).IsRequired();
        builder.Property(cart => cart.ProductId).IsRequired();

        builder.HasIndex(cart => new { cart.UserId, cart.ProductId }).IsUnique();

        builder.HasOne(cart => cart.User)
            .WithMany()
            .HasForeignKey(cart => cart.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cart => cart.Product)
            .WithMany(product => product.Carts)
            .HasForeignKey(cart => cart.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(wishlist => wishlist.Product.Status != ProductStatus.Disabled);
    }
}
