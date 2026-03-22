using SharedKernel.CQRS;
using Shopping.Application.Aggregates.OrderAggregate.Responses;

namespace Shopping.Application.Aggregates.OrderAggregate.Queries.GetOrderById;

public record GetOrderByIdQuery(int OrderId) : IQuery<OrderResponse>;
