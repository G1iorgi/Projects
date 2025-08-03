using DigitalBank.API.DB;
using DigitalBank.API.DB.Entities;

namespace DigitalBank.API.Repositories;

public sealed class TransactionRepository(DigitalBankDbContext dbContext) : BaseRepository(dbContext)
{
    public async Task<Transaction> FindAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        return await DbContext.Transactions.FindAsync([transactionId], cancellationToken);
    }

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        await base.AddAsync(transaction, cancellationToken);

        await SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        Update(transaction);

        await SaveChangesAsync(cancellationToken);
    }
}
