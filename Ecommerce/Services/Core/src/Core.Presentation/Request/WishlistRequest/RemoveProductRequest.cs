using Ardalis.GuardClauses;
using Core.Application.Aggregates.WishlistAggregate.Commands;

namespace Core.Presentation.Request.WishlistRequest;

public record RemoveProductRequest
{
    public required int Id { get; init; }

    public static RemoveProductCommand ToCommand(RemoveProductRequest? request, string userId)
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
