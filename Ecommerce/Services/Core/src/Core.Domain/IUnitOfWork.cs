using Core.Domain.Aggregates.CategoryAggregate;
using Core.Domain.Aggregates.IdentityAggregate;
using Core.Domain.Aggregates.ProductAggregate;

namespace Core.Domain;

public interface IUnitOfWork
{
    ICategoryRepository Categories { get; }

    IProductRepository Products { get; }

    IRefreshTokenRepository RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
