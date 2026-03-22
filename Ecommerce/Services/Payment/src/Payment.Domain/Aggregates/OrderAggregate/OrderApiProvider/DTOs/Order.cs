namespace Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider.DTOs;

public record Order(int Id,
    string UserId,
    DateTimeOffset CreateDate,
    decimal TotalPrice,
    Guid TransactionId,
    OrderStatus Status,
    List<OrderItem> OrderItems);
