using Ardalis.GuardClauses;
using DigitalBank.API.DB.Entities;
using DigitalBank.API.DB.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.API.DB;

public class DigitalBankDbContext(DbContextOptions<DigitalBankDbContext> options) : DbContext(options)
{
    public const string MigrationHistoryTable = "__DigitalBankMigrationHistory";
    private const string DefaultSchema = "DigitalBank";

    public DbSet<CreditCard> CreditCards { get; set; }

    public DbSet<Balance> Balances { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Guard.Against.Null(modelBuilder);

        modelBuilder.HasDefaultSchema(DefaultSchema);
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CreditCardEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BalanceEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionEntityTypeConfiguration());
    }
}
