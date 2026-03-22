using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;

namespace Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider;

public interface IProductApiProvider
{
    public Task<Product?> GetProductByIdAsync(string jwt,
        int productId,
        CancellationToken cancellationToken = default);
}
