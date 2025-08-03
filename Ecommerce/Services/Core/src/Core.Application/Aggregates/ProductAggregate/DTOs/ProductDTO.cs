using Core.Domain.Aggregates.ProductAggregate;

namespace Core.Application.Aggregates.ProductAggregate.DTOs;

public record ProductDTO(
    int Id,
    string Name,
    string Barcode,
    string Description,
    decimal Price,
    string? Image,
    DateTimeOffset CreateDate,
    ProductStatus Status,
    int CategoryId,
    string CategoryName);
