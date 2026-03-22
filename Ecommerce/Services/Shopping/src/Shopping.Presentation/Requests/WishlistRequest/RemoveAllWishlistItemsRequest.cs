using Ardalis.GuardClauses;
using Shopping.Application.Aggregates.WishlistAggregate.Commands.RemoveAllItems;

namespace Shopping.Presentation.Requests.WishlistRequest;

public record RemoveAllWishlistItemsRequest
{
    public static RemoveAllWishlistItemsCommand ToCommand(string userId)
    {
        Guard.Against.NullOrWhiteSpace(userId);

        return new RemoveAllWishlistItemsCommand(UserId: userId);
    }
}
