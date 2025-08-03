using DigitalBank.API.DB;
using DigitalBank.API.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.API.Repositories;

public sealed class CreditCardRepository(DigitalBankDbContext dbContext) : BaseRepository(dbContext)
{
    public async Task<bool> ExistsAsync(
        string creditCardNumber,
        string expirationDate,
        string cvv,
        CancellationToken cancellationToken)
    {
        return await DbContext.CreditCards
            .AnyAsync(
                c =>
                    c.Number == creditCardNumber &&
                    c.ExpirationDate == expirationDate &&
                    c.CVV == cvv,
                cancellationToken: cancellationToken);
    }

    public async Task<CreditCard> GetAsync(
        string number,
        string expirationDate,
        string cvv,
        CancellationToken cancellationToken)
    {
        return await DbContext.CreditCards
            .FirstOrDefaultAsync(
                c =>
                    c.Number == number &&
                    c.ExpirationDate == expirationDate &&
                    c.CVV == cvv,
                cancellationToken: cancellationToken);
    }
}
