using Core.Application.Aggregates.ProductAggregate.DTOs;

namespace Core.Application.Aggregates.ProductAggregate.Commands;

public record DecreaseProductsQuantityCommand
{
    public required IReadOnlyList<ProductQuantityDto> Items { get; init; } = [];
}
