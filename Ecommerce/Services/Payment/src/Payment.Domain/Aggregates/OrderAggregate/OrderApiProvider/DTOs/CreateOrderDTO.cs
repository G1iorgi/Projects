namespace Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider.DTOs;

public record CreateOrderDTO
{
    public required string UserId { get; init; }

    public required decimal TotalPrice { get; init; }

    public required Guid TransactionId { get; init; }

    public required OrderStatus Status { get; init; }

    public required List<CreateOrderItemDTO> OrderItems { get; init; }
}
