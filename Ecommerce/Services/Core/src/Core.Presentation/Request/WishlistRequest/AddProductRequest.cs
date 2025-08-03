using Ardalis.GuardClauses;
using Core.Application.Aggregates.WishlistAggregate.Commands;

namespace Core.Presentation.Request.WishlistRequest;

public record AddProductRequest
{
    public required int Id { get; init; }

    public static AddProductCommand ToCommand(AddProductRequest? request, string userId)
    {
        Guard.Against.Null(request);
        Guard.Against.NegativeOrZero(request.Id);
        Guard.Against.NullOrWhiteSpace(userId);
        return new AddProductCommand
        {
            ProductId = request.Id,
            UserId = userId
        };
    }
}
