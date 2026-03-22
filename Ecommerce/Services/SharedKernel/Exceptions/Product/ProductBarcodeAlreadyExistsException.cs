using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Product;

public sealed class ProductBarcodeAlreadyExistsException(string barcode) : ConflictException(
    $"Product with barcode {barcode} already exists.",
    "PRODUCT_BARCODE_ALREADY_EXISTS");
