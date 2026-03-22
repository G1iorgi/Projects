namespace Core.Application.Aggregates.ProductAggregate.DTOs;

public record ProductQuantityDto
{
    public required int ProductId { get; init; }

    public required int Quantity { get; init; }
}
