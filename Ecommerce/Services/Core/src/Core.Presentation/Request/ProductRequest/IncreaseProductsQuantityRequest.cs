using Ardalis.GuardClauses;
using Core.Application.Aggregates.ProductAggregate.Commands;
using Core.Application.Aggregates.ProductAggregate.DTOs;

namespace Core.Presentation.Request.ProductRequest;

public record IncreaseProductsQuantityRequest
{
    public required IReadOnlyList<ProductQuantityRequest> Items { get; init; } = [];

    public static IncreaseProductsQuantityCommand ToCommand(IncreaseProductsQuantityRequest? request)
    {
        Guard.Against.Null(request);

        return new IncreaseProductsQuantityCommand
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
