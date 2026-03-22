using Ardalis.GuardClauses;
using Payment.Application.Aggregates.PaymentAggregate.Commands.PayFromCart;
using Payment.Domain.Aggregates.PaymentAggregate;

namespace Payment.Presentation.Requests;

public record PayFromCartRequest
{
    public required string CreditCardNumber { get; init; }

    public required string ExpirationDate { get; init; }

    public required string CVV { get; init; }

    public required CurrencyType Currency { get; init; }

    public static PayFromCartCommand ToCommand(PayFromCartRequest? request, string userId, string jwt)
    {
        Guard.Against.NullOrWhiteSpace(userId);

        return new PayFromCartCommand(
            UserId: userId,
            Jwt: jwt,
            CreditCardNumber: request.CreditCardNumber,
            ExpirationDate: request.ExpirationDate,
            CVV: request.CVV,
            Currency: request.Currency);
    }
}

