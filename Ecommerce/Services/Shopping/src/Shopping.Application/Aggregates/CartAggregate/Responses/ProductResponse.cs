namespace Shopping.Application.Aggregates.CartAggregate.Responses;

public record ProductResponse
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required string? Description { get; init; }

    public required decimal Price { get; init; }

    public required int Quantity { get; init; }

    public required string? Image { get; init; }
}
