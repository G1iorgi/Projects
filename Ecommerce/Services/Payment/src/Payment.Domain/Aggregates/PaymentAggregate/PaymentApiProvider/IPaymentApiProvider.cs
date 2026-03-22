using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider.DTOs;

namespace Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider;

public interface IPaymentApiProvider
{
    public Task<Guid> PayAsync(string creditCardNumber,
        string expirationDate,
        string cvv,
        CurrencyType currency,
        decimal amount,
        CancellationToken cancellationToken = default);

    public Task<Balance?> GetBalance(string creditCardNumber,
        string expirationDate,
        string cvv,
        CurrencyType currency,
        CancellationToken cancellationToken = default);

    public Task<Transaction?> GetTransaction(Guid transactionId,
        CancellationToken cancellationToken = default);

    public Task<TransactionStatus> GetTransactionStatus(Guid transactionId,
        CancellationToken cancellationToken = default);

    public Task<Guid> RefundAsync(Guid transactionId,
        CancellationToken cancellationToken = default);
}
