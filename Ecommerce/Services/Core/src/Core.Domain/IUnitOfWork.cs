using Core.Domain.Aggregates.CartAggregate;
using Core.Domain.Aggregates.CategoryAggregate;
using Core.Domain.Aggregates.IdentityAggregate;
using Core.Domain.Aggregates.ProductAggregate;
using Core.Domain.Aggregates.WishlistAggregate;

namespace Core.Domain;

public interface IUnitOfWork
{
    ICategoryRepository Categories { get; }

    IProductRepository Products { get; }

    IRefreshTokenRepository RefreshTokens { get; }

    IWishlistRepository Wishlists { get; }

    ICartRepository Carts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
