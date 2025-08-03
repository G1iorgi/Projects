using Ardalis.GuardClauses;
using DigitalBank.API.Enums;

namespace DigitalBank.API.DB.Entities;

public class Transaction
{
    public Guid Id { get; init; }

    public decimal Amount { get; private set; }

    public int BalanceId { get; private set; }

    public TransactionStatus Status { get; private set; }

    public Guid? PreviousTransactionId { get; private set; }

    public DateTimeOffset CreateDate { get; private set; }

    public virtual Balance Balance { get; private set; }

    public static Transaction Create(decimal amount, int balanceId, Guid? previousTransactionId = null)
    {
        Guard.Against.NegativeOrZero(amount);
        Guard.Against.NegativeOrZero(balanceId);

        return new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            BalanceId = balanceId,
            Status = TransactionStatus.Completed,
            CreateDate = DateTimeOffset.UtcNow,
            PreviousTransactionId = previousTransactionId
        };
    }

    public void Rollback()
    {
        Status = TransactionStatus.Refunded;
    }
}
