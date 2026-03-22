namespace Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;

public record GetProductsByIdsDto
{
    public required IReadOnlyList<int> ProductIds { get; init; } = new List<int>();
}

