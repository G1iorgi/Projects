using Core.Domain;
using Core.Domain.Aggregates.CartAggregate;
using Core.Domain.Aggregates.CategoryAggregate;
using Core.Domain.Aggregates.IdentityAggregate;
using Core.Domain.Aggregates.ProductAggregate;
using Core.Domain.Aggregates.WishlistAggregate;
using Core.Infrastructure.DbContexts;

namespace Core.Infrastructure.Repositories;

public class UnitOfWork(
    CoreDbContextMaster dbContext,
    ICategoryRepository categoryRepository,
    IProductRepository productRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IWishlistRepository wishlistRepository,
    ICartRepository cartRepository)
    : IUnitOfWork
{
    public ICategoryRepository Categories => categoryRepository;

    public IProductRepository Products => productRepository;

    public IRefreshTokenRepository RefreshTokens => refreshTokenRepository;

    public IWishlistRepository Wishlists => wishlistRepository;

    public ICartRepository Carts => cartRepository;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => await dbContext.SaveChangesAsync(cancellationToken);
}
