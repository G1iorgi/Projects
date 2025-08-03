using Core.Domain.Aggregates.CartAggregate;
using Core.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repositories;

public class CartRepository(CoreDbContextMaster dbContext)
    : GenericRepository<Cart>(dbContext), ICartRepository
{
    public async Task<IEnumerable<Cart>> GetCartItemsAsync(
        string userId,
        CancellationToken cancellationToken = default)
        => await CoreDbContext.Carts
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task<Cart?> GetCartItemAsync(
        string userId,
        int productId,
        CancellationToken cancellationToken = default)
        => await CoreDbContext.Carts
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId, cancellationToken);
}
