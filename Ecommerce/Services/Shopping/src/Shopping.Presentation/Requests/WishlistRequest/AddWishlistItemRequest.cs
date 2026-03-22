using Ardalis.GuardClauses;
using Shopping.Application.Aggregates.WishlistAggregate.Commands.AddItem;

namespace Shopping.Presentation.Requests.WishlistRequest;

public record AddWishlistItemRequest
{
    public required int ProductId { get; init; }

    public static AddWishlistItemCommand ToCommand(AddWishlistItemRequest? request, string userId, string jwt)
    {
        Guard.Against.NullOrWhiteSpace(userId);

        return new AddWishlistItemCommand(
            UserId: userId,
            Jwt: jwt,
            ProductId: request.ProductId);
    }
}
