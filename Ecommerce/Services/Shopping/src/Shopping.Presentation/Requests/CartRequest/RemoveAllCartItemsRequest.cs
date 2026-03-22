using Ardalis.GuardClauses;
using Shopping.Application.Aggregates.CartAggregate.Commands.RemoveAllItems;

namespace Shopping.Presentation.Requests.CartRequest;

public record RemoveAllCartItemsRequest
{
    public static RemoveAllCartItemsCommand ToCommand(string userId)
    {
        Guard.Against.NullOrWhiteSpace(userId);

        return new RemoveAllCartItemsCommand(userId);
    }
}
