namespace Core.Application.Aggregates.CartAggregate.Responses;

public record ProductResponse
{
    public required string Name { get; init; }

    public required string? Description { get; init; }

    public required decimal Price { get; init; }

    public required string? Image { get; init; }
}
