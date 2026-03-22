using Core.Domain;
using Core.Domain.Aggregates.CategoryAggregate;
using Core.Domain.Aggregates.IdentityAggregate;
using Core.Domain.Aggregates.ProductAggregate;
using Core.Infrastructure.DbContexts;

namespace Core.Infrastructure.Repositories;

public class UnitOfWork(
    CoreDbContextMaster dbContext,
    ICategoryRepository categoryRepository,
    IProductRepository productRepository,
    IRefreshTokenRepository refreshTokenRepository)
    : IUnitOfWork
{
    public ICategoryRepository Categories => categoryRepository;

    public IProductRepository Products => productRepository;

    public IRefreshTokenRepository RefreshTokens => refreshTokenRepository;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => await dbContext.SaveChangesAsync(cancellationToken);
}
