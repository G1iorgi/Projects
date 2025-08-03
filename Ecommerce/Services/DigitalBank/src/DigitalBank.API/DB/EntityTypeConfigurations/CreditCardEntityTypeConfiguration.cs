using DigitalBank.API.DB.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalBank.API.DB.EntityTypeConfigurations;

internal class CreditCardEntityTypeConfiguration : IEntityTypeConfiguration<CreditCard>
{
    public void Configure(EntityTypeBuilder<CreditCard> builder)
    {
        builder.ToTable("CreditCards");

        builder.HasKey(creditCard => creditCard.Id);
        builder.Property(creditCard => creditCard.Id).ValueGeneratedOnAdd();
        builder.Property(creditCard => creditCard.Number).IsRequired();
        builder.Property(creditCard => creditCard.ExpirationDate).IsRequired();
        builder.Property(creditCard => creditCard.CVV).IsRequired();
    }
}
