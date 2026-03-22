using Payment.Domain.Aggregates.PaymentAggregate;
using SharedKernel.CQRS;

namespace Payment.Application.Aggregates.PaymentAggregate.Commands.Pay;

public record PayCommand(
    string UserId,
    string Jwt,
    int ProductId,
    int Quantity,
    string CreditCardNumber,
    string ExpirationDate,
    string CVV,
    CurrencyType Currency) : ICommand;
