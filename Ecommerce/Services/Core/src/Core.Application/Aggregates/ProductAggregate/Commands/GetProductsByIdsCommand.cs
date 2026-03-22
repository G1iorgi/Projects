namespace Core.Application.Aggregates.ProductAggregate.Commands;

public record GetProductsByIdsCommand
{
    public required IReadOnlyList<int> ProductIds { get; init; } = new List<int>();
}
