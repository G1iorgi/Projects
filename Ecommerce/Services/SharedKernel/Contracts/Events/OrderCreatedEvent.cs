using SharedKernel.Contracts.Events.DTOs;

namespace SharedKernel.Contracts.Events;

public record OrderCreatedEvent(
    string UserId,
    DateTimeOffset OrderDate,
    decimal TotalPrice,
    Guid TransactionId,
    OrderStatuses Statuses,
    IReadOnlyList<OrderItemDTO> OrderItems);
