using DigitalBank.API.DB.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalBank.API.DB.EntityTypeConfigurations;

internal class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(transaction => transaction.Id);
        builder.Property(transaction => transaction.Id).ValueGeneratedOnAdd();
        builder.Property(transaction => transaction.Amount).IsRequired();
        builder.Property(transaction => transaction.Status).IsRequired();
        builder.Property(transaction => transaction.CreateDate).IsRequired();
        builder.Property(transaction => transaction.PreviousTransactionId).IsRequired(false);
    }
}
