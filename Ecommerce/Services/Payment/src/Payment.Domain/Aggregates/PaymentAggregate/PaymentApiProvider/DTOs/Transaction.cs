namespace Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider.DTOs;

public record Transaction(
    Guid Id,
    decimal Amount,
    string CreditCardNumber,
    TransactionStatus Status,
    DateTimeOffset CreateDate);
