using Ardalis.GuardClauses;
using Payment.Application.Aggregates.PaymentAggregate.Commands.Refund;

namespace Payment.Presentation.Requests;

public record RefundRequest
{
    public required int OrderId { get; init; }

    public static RefundCommand ToCommand(RefundRequest? request, string userId, string jwt)
    {
        Guard.Against.NullOrWhiteSpace(userId);
        Guard.Against.NullOrWhiteSpace(jwt);

        return new RefundCommand(
            UserId: userId,
            Jwt: jwt,
            OrderId: request.OrderId);
    }
}
