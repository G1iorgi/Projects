using Core.Domain.Aggregates.ProductAggregate;
using Core.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repositories;

public class ProductRepository(CoreDbContextMaster dbContext)
    : GenericRepository<Product>(dbContext), IProductRepository
{
    public async Task<bool> IsUniqueAsync(string barcode,
        CancellationToken cancellationToken = default)
        => !await CoreDbContext.Products.AnyAsync(product => product.Barcode == barcode, cancellationToken);

    public Task<List<Product>> GetByIdsAsync(IReadOnlyList<int> ids,
        CancellationToken cancellationToken = default)
        => CoreDbContext.Products.Where(product => ids.Contains(product.Id)).ToListAsync(cancellationToken);
}
