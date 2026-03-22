using Ardalis.GuardClauses;
using Shopping.Application.Aggregates.WishlistAggregate.Commands.RemoveItem;

namespace Shopping.Presentation.Requests.WishlistRequest;

public record RemoveWishlistItemRequest
{
    public required int ProductId { get; init; }

    public static RemoveWishlistItemCommand ToCommand(RemoveWishlistItemRequest? request, string userId)
    {
        Guard.Against.NullOrWhiteSpace(userId);

        return new RemoveWishlistItemCommand(ProductId: request.ProductId, UserId: userId);
    }
}
