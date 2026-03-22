using Core.Domain.Aggregates.ProductAggregate;
using Core.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.EntityConfigurations;

internal sealed class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(nameof(Product), CoreDbContextMaster.DefaultSchema);

        builder.HasKey(product => product.Id);
        builder.Property(product => product.Id).ValueGeneratedOnAdd();

        builder.Property(product => product.Name).IsRequired();
        builder.HasIndex(product => product.Name);

        builder.Property(product => product.Barcode).IsRequired();
        builder.HasIndex(product => product.Barcode).IsUnique();

        builder.Property(product => product.Price).IsRequired();

        builder.Property(product => product.Status).IsRequired();

        builder.Property(product => product.CreateDate).IsRequired();

        builder.HasQueryFilter(product => product.Status != ProductStatus.Disabled);

        // Explicitly define the relationship with category
        builder.HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
