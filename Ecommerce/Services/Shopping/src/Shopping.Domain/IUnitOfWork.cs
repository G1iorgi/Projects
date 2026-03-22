using Shopping.Domain.Aggregates.CartAggregate;
using Shopping.Domain.Aggregates.OrderAggregate;
using Shopping.Domain.Aggregates.WishlistAggregate;

namespace Shopping.Domain;

public interface IUnitOfWork
{
    ICartRepository Carts { get; }

    IWishlistRepository Wishlists { get; }

    IOrderRepository Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
