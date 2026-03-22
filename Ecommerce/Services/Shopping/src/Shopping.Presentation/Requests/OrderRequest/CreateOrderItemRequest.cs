namespace Shopping.Presentation.Requests.OrderRequest;

public record CreateOrderItemRequest(int ProductId,
    decimal Price,
    int Quantity);
