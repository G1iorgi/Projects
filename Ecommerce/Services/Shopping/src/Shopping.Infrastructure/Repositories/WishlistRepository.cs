using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Aggregates.WishlistAggregate;
using Shopping.Infrastructure.DbContexts;

namespace Shopping.Infrastructure.Repositories;

public class WishlistRepository(ShoppingDbContextMaster dbContext)
    : GenericRepository<Wishlist>(dbContext), IWishlistRepository
{
    public async Task<Wishlist?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        => await ShoppingDbContext.Wishlists
            .FirstOrDefaultAsync(wishlist => wishlist.UserId == userId, cancellationToken);
}
