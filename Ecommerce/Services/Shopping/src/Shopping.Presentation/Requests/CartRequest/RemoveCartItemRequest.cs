using Ardalis.GuardClauses;
using Shopping.Application.Aggregates.CartAggregate.Commands.RemoveItem;

namespace Shopping.Presentation.Requests.CartRequest;

public record RemoveCartItemRequest
{
    public required int ProductId { get; init; }

    public static RemoveCartItemCommand ToCommand(RemoveCartItemRequest? request, string userId)
    {
        Guard.Against.NullOrWhiteSpace(userId);

        return new RemoveCartItemCommand(request.ProductId, userId);
    }
}
