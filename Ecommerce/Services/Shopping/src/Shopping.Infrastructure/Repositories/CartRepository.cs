using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Aggregates.CartAggregate;
using Shopping.Infrastructure.DbContexts;

namespace Shopping.Infrastructure.Repositories;

public class CartRepository(ShoppingDbContextMaster dbContext)
    : GenericRepository<Cart>(dbContext), ICartRepository
{
    public async Task<Cart?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        => await ShoppingDbContext.Carts
            .FirstOrDefaultAsync(cart => cart.UserId == userId, cancellationToken);
}
