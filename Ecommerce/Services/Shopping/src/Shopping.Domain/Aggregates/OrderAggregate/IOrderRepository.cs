namespace Shopping.Domain.Aggregates.OrderAggregate;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default);
}
