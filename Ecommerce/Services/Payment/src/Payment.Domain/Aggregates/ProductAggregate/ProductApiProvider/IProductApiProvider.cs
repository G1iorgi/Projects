using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;

namespace Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider;

public interface IProductApiProvider
{
    public Task<List<Product>?> GetProductsByIdsAsync(string jwt,
        GetProductsByIdsDto dto,
        CancellationToken cancellationToken = default);

    public Task<Product?> GetProductByIdAsync(string jwt, int productId,
        CancellationToken cancellationToken = default);

    public Task DecreaseProductsQuantityAsync(string jwt,
        DecreaseProductQuantitiesDto dto,
        CancellationToken cancellationToken = default);

    public Task IncreaseProductsQuantityAsync(string jwt,
        IncreaseProductQuantitiesDto dto,
        CancellationToken cancellationToken = default);
}
