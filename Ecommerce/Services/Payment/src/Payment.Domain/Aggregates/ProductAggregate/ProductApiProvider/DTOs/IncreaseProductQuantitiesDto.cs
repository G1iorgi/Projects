namespace Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;

public record IncreaseProductQuantitiesDto
{
    public IReadOnlyList<ProductQuantityDto> Items { get; init; } = [];
}
