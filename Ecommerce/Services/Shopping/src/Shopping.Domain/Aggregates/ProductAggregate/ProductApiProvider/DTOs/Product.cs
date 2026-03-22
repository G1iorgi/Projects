namespace Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;

public record Product(
    int Id,
    string Name,
    string Barcode,
    string Description,
    decimal Price,
    string? Image,
    int Quantity,
    DateTimeOffset CreateDate,
    ProductStatus Status,
    int CategoryId,
    string CategoryName);
