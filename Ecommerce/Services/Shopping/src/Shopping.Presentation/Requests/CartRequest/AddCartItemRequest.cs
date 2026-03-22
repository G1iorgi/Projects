using Ardalis.GuardClauses;
using Shopping.Application.Aggregates.CartAggregate.Commands.AddItem;

namespace Shopping.Presentation.Requests.CartRequest;

public record AddCartItemRequest
{
    public required int ProductId { get; init; }

    public required int ProductQuantity { get; init; }

    public static AddCartItemCommand ToCommand(AddCartItemRequest? request, string userId, string jwt)
    {
        Guard.Against.NullOrWhiteSpace(userId);

        return new AddCartItemCommand(
            UserId: userId,
            Jwt: jwt,
            ProductId: request.ProductId,
            ProductQuantity: request.ProductQuantity);
    }
}
