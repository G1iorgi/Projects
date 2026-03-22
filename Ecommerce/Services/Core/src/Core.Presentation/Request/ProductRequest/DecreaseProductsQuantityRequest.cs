using Ardalis.GuardClauses;
using Core.Application.Aggregates.ProductAggregate.Commands;
using Core.Application.Aggregates.ProductAggregate.DTOs;

namespace Core.Presentation.Request.ProductRequest;

public record DecreaseProductsQuantityRequest
{
    public required IReadOnlyList<ProductQuantityRequest> Items { get; init; } = [];

    public static DecreaseProductsQuantityCommand ToCommand(DecreaseProductsQuantityRequest? request)
    {
        Guard.Against.Null(request);

        return new DecreaseProductsQuantityCommand
        {
            Items = request.Items
                .Select(item => new ProductQuantityDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                })
                .ToList()
        };
    }
}
