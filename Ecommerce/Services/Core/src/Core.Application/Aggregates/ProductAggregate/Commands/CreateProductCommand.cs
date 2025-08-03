namespace Core.Application.Aggregates.ProductAggregate.Commands;

public record CreateProductCommand
{
    public required string Name { get; init; }

    public required string Barcode { get; init; }

    public string? Description { get; init; }

    public required decimal Price { get; init; }

    public string? Image { get; init; }

    public required int CategoryId { get; init; }
}
