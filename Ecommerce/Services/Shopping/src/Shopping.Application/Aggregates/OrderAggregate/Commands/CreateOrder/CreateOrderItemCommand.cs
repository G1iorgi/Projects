namespace Shopping.Application.Aggregates.OrderAggregate.Commands.CreateOrder;

public record CreateOrderItemCommand(int ProductId,
    decimal Price,
    int Quantity);
