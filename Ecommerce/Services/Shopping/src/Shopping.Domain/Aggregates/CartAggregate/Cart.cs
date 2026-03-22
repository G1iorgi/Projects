using Ardalis.GuardClauses;
using SharedKernel.Exceptions.Product;

namespace Shopping.Domain.Aggregates.CartAggregate;

public class Cart
{
    public Cart()
    {
    }

    private Cart(string userId)
    {
        UserId = userId;
    }

    public int Id { get; init; }

    public string UserId { get; private set; }

    public decimal TotalPrice => CartItems.Sum(x => x.ProductPrice * x.ProductQuantity);

    public virtual ICollection<CartItem> CartItems { get; private set; } = new List<CartItem>();

    public static Cart Create(string userId)
    {
        Guard.Against.NullOrWhiteSpace(userId);
        return new Cart(userId);
    }

    public void AddItem(int productId, string productName, int quantity, decimal price)
    {
        var existingItem = CartItems.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            CartItems.Add(CartItem.Create(Id, productId, productName, quantity, price));
        }
    }

    public void RemoveItem(int productId)
    {
        var item = CartItems.FirstOrDefault(x => x.ProductId == productId);
        if (item is null)
            throw new ProductNotFoundException(productId);

        CartItems.Remove(item);
    }
}
