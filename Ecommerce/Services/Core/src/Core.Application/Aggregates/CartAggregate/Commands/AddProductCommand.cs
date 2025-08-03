namespace Core.Application.Aggregates.CartAggregate.Commands;

public record AddProductCommand
{
    public required int ProductId { get; init; }

    public required string UserId { get; init; }
}
