namespace SharedKernel.Contracts.Events.DTOs;

public record OrderItemDTO(
    int ProductId,
    int Quantity,
    decimal Price);
