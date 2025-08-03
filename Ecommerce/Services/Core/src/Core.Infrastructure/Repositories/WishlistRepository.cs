using Core.Domain.Aggregates.WishlistAggregate;
using Core.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repositories;

public class WishlistRepository(CoreDbContextMaster dbContext)
    : GenericRepository<Wishlist>(dbContext), IWishlistRepository
{
    public async Task<IEnumerable<Wishlist>> GetWishlistItemsAsync(
        string userId,
        CancellationToken cancellationToken = default)
        => await CoreDbContext.Wishlists
            .Where(w => w.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task<Wishlist?> GetWishlistItemAsync(
        string userId,
        int productId,
        CancellationToken cancellationToken = default)
        => await CoreDbContext.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId, cancellationToken);
}
