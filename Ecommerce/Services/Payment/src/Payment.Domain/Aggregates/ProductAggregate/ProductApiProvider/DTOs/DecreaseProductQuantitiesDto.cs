namespace Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;

public record DecreaseProductQuantitiesDto
{
    public IReadOnlyList<ProductQuantityDto> Items { get; init; } = [];
}
