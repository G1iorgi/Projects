namespace Core.Presentation.Request.ProductRequest;

public record ProductQuantityRequest
{
    public required int ProductId { get; init; }

    public required int Quantity { get; init; }
}
