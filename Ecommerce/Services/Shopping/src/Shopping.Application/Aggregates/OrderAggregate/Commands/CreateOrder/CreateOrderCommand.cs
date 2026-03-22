using SharedKernel.CQRS;
using Shopping.Application.Aggregates.OrderAggregate.Responses;
using Shopping.Domain.Aggregates.OrderAggregate;

namespace Shopping.Application.Aggregates.OrderAggregate.Commands.CreateOrder;

public record CreateOrderCommand(string UserId,
    decimal TotalPrice,
    Guid TransactionId,
    OrderStatus Status,
    List<CreateOrderItemCommand> OrderItems) : ICommand<OrderResponse>;
