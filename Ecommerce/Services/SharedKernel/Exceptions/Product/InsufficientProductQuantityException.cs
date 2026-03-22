using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Product;

public sealed class InsufficientProductQuantityException(int productId) : ConflictException(
    $"Not enough quantity for product with ID {productId}.",
    "INSUFFICIENT_PRODUCT_QUANTITY");
