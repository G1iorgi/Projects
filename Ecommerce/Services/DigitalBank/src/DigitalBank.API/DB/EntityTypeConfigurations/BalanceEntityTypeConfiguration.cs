using DigitalBank.API.DB.Entities;
using DigitalBank.API.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalBank.API.DB.EntityTypeConfigurations;

internal class BalanceEntityTypeConfiguration : IEntityTypeConfiguration<Balance>
{
    public void Configure(EntityTypeBuilder<Balance> builder)
    {
        builder.ToTable("Balances");

        builder.HasKey(balance => balance.Id);
        builder.Property(balance => balance.Id).ValueGeneratedOnAdd();

        var converter = new ValueConverter<CurrencyType, string>(
            currencyType => currencyType.ToString(),
            stringValue => (CurrencyType)Enum.Parse(typeof(CurrencyType), stringValue));
        builder.Property(balance => balance.Currency).IsRequired().HasConversion(converter);

        builder.Property(balance => balance.Amount).IsRequired();
        builder.Property(balance => balance.CreditCardId).IsRequired();
    }
}
