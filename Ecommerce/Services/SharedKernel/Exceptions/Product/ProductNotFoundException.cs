using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Product;

public sealed class ProductNotFoundException : NotFoundException
{
    public ProductNotFoundException(int productId)
        : base($"Product with ID {productId} was not found.",
            "PRODUCT_NOT_FOUND")
    {
    }

    public ProductNotFoundException(IEnumerable<int> productIds)
        : base($"Products with IDs {string.Join(", ", productIds)} were not found.",
            "PRODUCTS_NOT_FOUND")
    {
    }
}
