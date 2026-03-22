namespace Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider.DTOs;

public record OrderItem(int Id,
    int OrderId,
    int ProductId,
    int Quantity,
    decimal Price);
