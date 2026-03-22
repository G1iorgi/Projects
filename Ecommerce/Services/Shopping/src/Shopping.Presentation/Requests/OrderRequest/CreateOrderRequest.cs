using Ardalis.GuardClauses;
using Shopping.Application.Aggregates.OrderAggregate.Commands.CreateOrder;
using Shopping.Domain.Aggregates.OrderAggregate;

namespace Shopping.Presentation.Requests.OrderRequest;

public record CreateOrderRequest
{
    public required decimal TotalPrice { get; init; }

    public required Guid TransactionId { get; init; }

    public required OrderStatus Status { get; init; }

    public required List<CreateOrderItemRequest> OrderItems { get; init; }

    public static CreateOrderCommand ToCommand(CreateOrderRequest request, string userId)
    {
        Guard.Against.NullOrWhiteSpace(userId);

        var orderItems = request.OrderItems
            .Select(oi => new CreateOrderItemCommand(
                ProductId: oi.ProductId,
                Price: oi.Price,
                Quantity: oi.Quantity))
            .ToList();

        return new CreateOrderCommand(
            UserId: userId,
            TotalPrice: request.TotalPrice,
            TransactionId: request.TransactionId,
            Status: request.Status,
            OrderItems: orderItems);
    }
}
