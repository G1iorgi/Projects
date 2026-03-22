using Ardalis.GuardClauses;
using SharedKernel.Exceptions.Product;

namespace Shopping.Domain.Aggregates.WishlistAggregate;

public class Wishlist
{
    public Wishlist()
    {
    }

    private Wishlist(string userId)
    {
        UserId = userId;
    }

    public int Id { get; init; }

    public string UserId { get; private set; }

    public virtual ICollection<WishlistItem> WishlistItems { get; private set; } = new List<WishlistItem>();

    public static Wishlist Create(string userId)
    {
        Guard.Against.NullOrWhiteSpace(userId);
        return new Wishlist(userId);
    }

    public void AddItem(int productId, string productName, decimal price)
    {
        WishlistItems.Add(WishlistItem.Create(Id, productId, productName, price));
    }

    public void RemoveItem(int productId)
    {
        var item = WishlistItems.FirstOrDefault(x => x.ProductId == productId);
        if (item is null)
            throw new ProductNotFoundException(productId);

        WishlistItems.Remove(item);
    }
}
