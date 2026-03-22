using Shopping.Domain;
using Shopping.Domain.Aggregates.CartAggregate;
using Shopping.Domain.Aggregates.OrderAggregate;
using Shopping.Domain.Aggregates.WishlistAggregate;
using Shopping.Infrastructure.DbContexts;

namespace Shopping.Infrastructure.Repositories;

public class UnitOfWork(
    ShoppingDbContextMaster dbContext,
    ICartRepository cartRepository,
    IWishlistRepository wishlistRepository,
    IOrderRepository orderRepository)
    : IUnitOfWork
{
    public ICartRepository Carts => cartRepository;

    public IWishlistRepository Wishlists => wishlistRepository;

    public IOrderRepository Orders => orderRepository;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
