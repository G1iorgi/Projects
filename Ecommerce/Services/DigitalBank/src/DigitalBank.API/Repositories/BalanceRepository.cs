using DigitalBank.API.DB;
using DigitalBank.API.DB.Entities;

namespace DigitalBank.API.Repositories;

public sealed class BalanceRepository(DigitalBankDbContext dbContext) : BaseRepository(dbContext)
{
    public async Task UpdateAsync(Balance balance, CancellationToken cancellationToken)
    {
        Update(balance);

        await SaveChangesAsync(cancellationToken);
    }
}
