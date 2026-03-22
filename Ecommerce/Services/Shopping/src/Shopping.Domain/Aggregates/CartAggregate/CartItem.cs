using Ardalis.GuardClauses;

namespace Shopping.Domain.Aggregates.CartAggregate;

public class CartItem
{
    protected CartItem()
    {
    }

    public CartItem(int cartId, int productId, string productName, int productQuantity, decimal productprice)
    {
        CartId = cartId;
        ProductId = productId;
        ProductName = productName;
        ProductQuantity = productQuantity;
        ProductPrice = productprice;
    }

    public int CartId { get; private set; }

    public int ProductId { get; private set; }

    public string ProductName { get; private set; }

    public string? ProductDescription { get; private set; }

    public string? ProductImage { get; private set; }

    public int ProductQuantity { get; private set; }

    public decimal ProductPrice { get; private set; }

    public virtual Cart Cart { get; private set; }

    public static CartItem Create(int cartId,
        int productId,
        string productName,
        int productQuantity,
        decimal productPrice)
    {
        Guard.Against.NullOrWhiteSpace(productName);
        Guard.Against.NegativeOrZero(productQuantity);
        Guard.Against.NegativeOrZero(productPrice);

        return new CartItem(cartId, productId, productName, productQuantity, productPrice);
    }

    public void IncreaseQuantity(int productQuantity)
    {
        Guard.Against.NegativeOrZero(productQuantity);
        ProductQuantity += productQuantity;
    }
}
