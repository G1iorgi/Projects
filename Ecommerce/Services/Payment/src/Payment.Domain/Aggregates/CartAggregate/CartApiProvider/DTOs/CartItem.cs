namespace Payment.Domain.Aggregates.CartAggregate.CartApiProvider.DTOs;

public record CartItem
{
    public int Id { get; init; }

    public int Quantity { get; init; }
}
