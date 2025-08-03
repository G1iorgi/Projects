using Ardalis.GuardClauses;
using Core.Application.Aggregates.CartAggregate.Commands;

namespace Core.Presentation.Request.CartRequest;

public class RemoveProductFromCartRequest
{
    public required int Id { get; init; }

    public static RemoveProductCommand ToCommand(RemoveProductFromCartRequest? request, string userId)
    {
        Guard.Against.Null(request);
        Guard.Against.NegativeOrZero(request.Id);
        Guard.Against.NullOrWhiteSpace(userId);

        return new RemoveProductCommand
        {
            ProductId = request.Id,
            UserId = userId
        };
    }
}
