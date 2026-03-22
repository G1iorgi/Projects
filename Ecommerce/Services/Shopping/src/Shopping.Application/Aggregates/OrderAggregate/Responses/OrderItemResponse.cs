namespace Shopping.Application.Aggregates.OrderAggregate.Responses;

public record OrderItemResponse(int ProductId, int Quantity, decimal Price);
