using Ardalis.GuardClauses;
using Payment.Application.Aggregates.PaymentAggregate.Commands.Pay;
using Payment.Domain.Aggregates.PaymentAggregate;

namespace Payment.Presentation.Requests;

public record PayRequest
{
    public required int ProductId { get; init; }

    public required int Quantity { get; init; }

    public required string CreditCardNumber { get; init; }

    public required string ExpirationDate { get; init; }

    public required string CVV { get; init; }

    public required CurrencyType Currency { get; init; }

    public static PayCommand ToCommand(PayRequest? request, string userId, string jwt)
    {
        Guard.Against.NullOrWhiteSpace(userId);

        return new PayCommand(
            UserId: userId,
            ProductId: request.ProductId,
            Quantity: request.Quantity,
            CreditCardNumber: request.CreditCardNumber,
            ExpirationDate: request.ExpirationDate,
            CVV: request.CVV,
            Currency: request.Currency,
            Jwt: jwt);
    }
}
