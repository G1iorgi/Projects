using Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider.DTOs;

namespace Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider;

public interface IOrderApiProvider
{
    public Task<Order?> GetOrderByIdAsync(string jwt, int orderId, CancellationToken cancellationToken = default);
}
