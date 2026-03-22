using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Aggregates.CartAggregate;
using Shopping.Infrastructure.DbContexts;

namespace Shopping.Infrastructure.EntityConfigurations;

internal sealed class CartEntityTypeConfiguration
    : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable(nameof(Cart), ShoppingDbContextMaster.DefaultSchema);

        builder.HasKey(cart => cart.Id);

        builder.Property(cart => cart.UserId).IsRequired();
    }
}
