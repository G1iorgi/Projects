using Ardalis.GuardClauses;

namespace Shopping.Domain.Aggregates.OrderAggregate;

public class Order
{
    public Order()
    {
    }

    private Order(string userId, DateTimeOffset orderDate, decimal totalPrice, Guid transactionId)
    {
        UserId = userId;
        OrderDate = orderDate;
        TotalPrice = totalPrice;
        TransactionId = transactionId;
        OrderItems = new List<OrderItem>();
    }

    public int Id { get; init; }

    public string UserId { get; private set; }

    public DateTimeOffset OrderDate { get; private set; }

    public decimal TotalPrice { get; private set; }

    public Guid TransactionId { get; private set; }

    public OrderStatus Status { get; private set; }

    public virtual ICollection<OrderItem> OrderItems { get; private set; }

    public static Order Create(string userId,
        decimal totalPrice,
        Guid transactionId,
        OrderStatus orderStatus,
        List<OrderItem> orderItems)
    {
        Guard.Against.NullOrWhiteSpace(userId);
        Guard.Against.NegativeOrZero(totalPrice);
        Guard.Against.Default(transactionId);
        Guard.Against.Null(orderItems);

        var order = new Order
        {
            UserId = userId,
            TotalPrice = totalPrice,
            TransactionId = transactionId,
            OrderDate = DateTimeOffset.UtcNow,
            Status = orderStatus,
            OrderItems = orderItems
        };

        return order;
    }
}
