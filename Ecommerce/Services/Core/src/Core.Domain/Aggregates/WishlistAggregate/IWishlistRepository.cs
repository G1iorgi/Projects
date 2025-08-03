namespace Core.Domain.Aggregates.WishlistAggregate;

public interface IWishlistRepository : IGenericRepository<Wishlist>
{
    Task<IEnumerable<Wishlist>> GetWishlistItemsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Wishlist?> GetWishlistItemAsync(
        string userId,
        int productId,
        CancellationToken cancellationToken = default);
}
