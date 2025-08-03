using Ardalis.GuardClauses;
using Core.Domain.Aggregates.ProductAggregate;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Aggregates.CartAggregate;

public class Cart
{
    public Cart()
    {
    }

    private Cart(string userId, int productId)
    {
        UserId = userId;
        ProductId = productId;
    }

    public int Id { get; init; }

    public string UserId { get; private set; } = null!;

    public virtual IdentityUser User { get; private set; }

    public int ProductId { get; private set; }

    public virtual Product Product { get; private set; }

    public static Cart Create(string userId, int productId)
    {
        Guard.Against.NullOrWhiteSpace(userId);
        Guard.Against.NegativeOrZero(productId);

        return new Cart(userId, productId);
    }
}
