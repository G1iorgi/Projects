using Ardalis.GuardClauses;

namespace Shopping.Domain.Aggregates.OrderAggregate;

public class OrderItem
{
    public OrderItem()
    {
    }

    private OrderItem(int productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public int Id { get; init; }

    public int OrderId { get; private set; }

    public int ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public virtual Order Order { get; set; }

    public static OrderItem Create(int productId, int quantity, decimal unitPrice)
    {
        Guard.Against.NegativeOrZero(productId);
        Guard.Against.NegativeOrZero(quantity);
        Guard.Against.NegativeOrZero(unitPrice);

        return new OrderItem
        {
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}
