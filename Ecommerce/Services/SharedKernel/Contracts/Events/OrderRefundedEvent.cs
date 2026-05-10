using SharedKernel.Contracts.Events.DTOs;

namespace SharedKernel.Contracts.Events;

public record OrderRefundedEvent(
    string UserId,
    DateTimeOffset OrderDate,
    decimal TotalPrice,
    Guid TransactionId,
    OrderStatuses Statuses,
    IReadOnlyList<OrderItemDTO> OrderItems);
