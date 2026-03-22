namespace Core.Domain.Aggregates.ProductAggregate;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<bool> IsUniqueAsync(string barcode,
        CancellationToken cancellationToken = default);

    Task<List<Product>> GetByIdsAsync(IReadOnlyList<int> ids,
        CancellationToken cancellationToken = default);
}
