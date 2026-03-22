using Shopping.Domain.Aggregates.OrderAggregate;

namespace Shopping.Application.Aggregates.OrderAggregate.Responses;

public record OrderResponse(int OrderId,
    decimal TotalPrice,
    Guid TransactionId,
    OrderStatus Status,
    List<OrderItemResponse> OrderItems);
