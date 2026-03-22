using Ardalis.GuardClauses;
using Core.Application.Aggregates.ProductAggregate.Commands;

namespace Core.Presentation.Request.ProductRequest;

public record UpdateProductRequest
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required string Barcode { get; init; }

    public string? Description { get; init; }

    public required decimal Price { get; init; }

    public string? Image { get; init; }

    public required int Quantity { get; init; }

    public required int CategoryId { get; init; }

    public static UpdateProductCommand ToCommand(UpdateProductRequest? request)
    {
        Guard.Against.Null(request);
        Guard.Against.NegativeOrZero(request.Id);
        Guard.Against.NullOrWhiteSpace(request.Name);
        Guard.Against.NullOrWhiteSpace(request.Barcode);
        Guard.Against.NegativeOrZero(request.Price);
        Guard.Against.NegativeOrZero(request.CategoryId);
        Guard.Against.Negative(request.Quantity);

        return new UpdateProductCommand
        {
            Id = request.Id,
            Name = request.Name,
            Barcode = request.Barcode,
            Description = request.Description,
            Price = request.Price,
            Image = request.Image,
            Quantity = request.Quantity,
            CategoryId = request.CategoryId
        };
    }
}
