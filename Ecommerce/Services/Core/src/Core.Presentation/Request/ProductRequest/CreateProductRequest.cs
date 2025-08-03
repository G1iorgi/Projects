using Ardalis.GuardClauses;
using Core.Application.Aggregates.ProductAggregate.Commands;

namespace Core.Presentation.Request.ProductRequest;

public record CreateProductRequest
{
    public required string Name { get; init; }

    public required string Barcode { get; init; }

    public string? Description { get; init; }

    public required decimal Price { get; init; }

    public string? Image { get; init; }

    public required int CategoryId { get; init; }

    public static CreateProductCommand ToCommand(CreateProductRequest? request)
    {
        Guard.Against.Null(request);
        Guard.Against.NullOrWhiteSpace(request.Name);
        Guard.Against.NullOrWhiteSpace(request.Barcode);
        Guard.Against.NegativeOrZero(request.Price);
        Guard.Against.NegativeOrZero(request.CategoryId);

        return new CreateProductCommand
        {
            Name = request.Name,
            Barcode = request.Barcode,
            Description = request.Description,
            Price = request.Price,
            Image = request.Image,
            CategoryId = request.CategoryId
        };
    }
}
