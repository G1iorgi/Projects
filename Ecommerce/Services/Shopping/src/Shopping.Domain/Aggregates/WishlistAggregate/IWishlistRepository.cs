namespace Shopping.Domain.Aggregates.WishlistAggregate;

public interface IWishlistRepository : IGenericRepository<Wishlist>
{
    Task<Wishlist?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
