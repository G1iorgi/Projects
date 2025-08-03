using Core.Domain.Aggregates.CategoryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.EntityConfigurations;

internal sealed class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(category => category.Id);
        builder.Property(category => category.Id).ValueGeneratedOnAdd();

        builder.Property(category => category.Name).IsRequired();
        builder.HasIndex(category => category.Name);

        builder.Property(category => category.Status).IsRequired();

        builder.Property(category => category.CreateDate).IsRequired();

        builder.HasQueryFilter(category => category.Status != CategoryStatus.Disabled);

        // Explicitly define the relationship with products
        builder.HasMany(category => category.Products)
            .WithOne(product => product.Category)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
