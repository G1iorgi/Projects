using Shopping.Application.Aggregates.OrderAggregate.Queries.GetOrderById;

namespace Shopping.Presentation.Requests.OrderRequest;

public record GetOrderByIdRequest
{
    public required int OrderId { get; init; }

    public static GetOrderByIdQuery ToQuery(GetOrderByIdRequest request)
    {
        return new GetOrderByIdQuery(OrderId: request.OrderId);
    }
}
