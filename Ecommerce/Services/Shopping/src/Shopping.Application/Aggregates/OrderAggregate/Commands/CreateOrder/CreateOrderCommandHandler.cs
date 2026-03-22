using Ardalis.GuardClauses;
using SharedKernel.CQRS;
using Shopping.Application.Aggregates.OrderAggregate.Responses;
using Shopping.Domain;
using Shopping.Domain.Aggregates.OrderAggregate;

namespace Shopping.Application.Aggregates.OrderAggregate.Commands.CreateOrder;

public class CreateOrderCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<CreateOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);
        Guard.Against.NullOrEmpty(command.OrderItems);

        var orderItems = command.OrderItems
            .Select(oi => OrderItem.Create(oi.ProductId, oi.Quantity, oi.Price))
            .ToList();

        var order = Order.Create(command.UserId,
            command.TotalPrice,
            command.TransactionId,
            command.Status,
            orderItems);

        await unitOfWork.Orders.CreateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var orderItemsResponse = orderItems
            .Select(oi => new OrderItemResponse(oi.ProductId, oi.Quantity, oi.UnitPrice))
            .ToList();

        return new OrderResponse(order.Id,
            order.TotalPrice,
            order.TransactionId,
            order.Status,
            orderItemsResponse);
    }
}
