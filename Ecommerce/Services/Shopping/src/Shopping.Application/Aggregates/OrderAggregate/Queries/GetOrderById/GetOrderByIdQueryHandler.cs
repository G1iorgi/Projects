using Ardalis.GuardClauses;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Order;
using Shopping.Application.Aggregates.OrderAggregate.Responses;
using Shopping.Domain;

namespace Shopping.Application.Aggregates.OrderAggregate.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    public async Task<OrderResponse> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query);

        var order = await unitOfWork.Orders.GetByIdAsync(query.OrderId, cancellationToken);

        if (order == null)
            throw new OrderNotFoundException(query.OrderId);

        var orderItems = order.OrderItems
            .Select(oi => new OrderItemResponse(oi.ProductId, oi.Quantity, oi.UnitPrice))
            .ToList();

        return new OrderResponse(order.Id,
            order.TotalPrice,
            order.TransactionId,
            order.Status,
            orderItems);
    }
}
