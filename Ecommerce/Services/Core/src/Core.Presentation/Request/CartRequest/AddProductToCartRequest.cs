using Ardalis.GuardClauses;
using Core.Application.Aggregates.CartAggregate.Commands;

namespace Core.Presentation.Request.CartRequest;

public record AddProductToCartRequest
{
    public required int Id { get; init; }

    public static AddProductCommand ToCommand(AddProductToCartRequest? request, string userId)
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
