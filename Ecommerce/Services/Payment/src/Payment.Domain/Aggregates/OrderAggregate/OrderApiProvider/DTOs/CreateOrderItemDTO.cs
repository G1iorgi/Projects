namespace Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider.DTOs;

public record CreateOrderItemDTO
{
    public required int ProductId { get; init; }

    public required decimal Price { get; init; }

    public required int Quantity { get; init; }
}
