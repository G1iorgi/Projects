using Ardalis.GuardClauses;

namespace Shopping.Domain.Aggregates.WishlistAggregate;

public class WishlistItem
{
    protected WishlistItem()
    {
    }

    public WishlistItem(int wishlistId, int productId, string productName, decimal productPrice)
    {
        WishlistId = wishlistId;
        ProductId = productId;
        ProductName = productName;
        ProductPrice = productPrice;
    }

    public int WishlistId { get; private set; }

    public int ProductId { get; private set; }

    public string ProductName { get; private set; }

    public string? ProductDescription { get; private set; }

    public string? ProductImage { get; private set; }

    public decimal ProductPrice { get; private set; }

    public virtual Wishlist Wishlist { get; private set; }

    public static WishlistItem Create(int wishlistId,
        int productId,
        string productName,
        decimal productPrice)
    {
        Guard.Against.NullOrWhiteSpace(productName);
        Guard.Against.NegativeOrZero(productPrice);

        return new WishlistItem(wishlistId, productId, productName, productPrice);
    }
}
