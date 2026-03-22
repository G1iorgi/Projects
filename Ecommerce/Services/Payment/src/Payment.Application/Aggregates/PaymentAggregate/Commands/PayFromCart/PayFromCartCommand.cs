using Payment.Domain.Aggregates.PaymentAggregate;
using SharedKernel.CQRS;

namespace Payment.Application.Aggregates.PaymentAggregate.Commands.PayFromCart;

public record PayFromCartCommand(
    string UserId,
    string Jwt,
    string CreditCardNumber,
    string ExpirationDate,
    string CVV,
    CurrencyType Currency) : ICommand;
