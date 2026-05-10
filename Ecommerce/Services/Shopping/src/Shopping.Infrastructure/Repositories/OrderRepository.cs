using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Aggregates.OrderAggregate;
using Shopping.Infrastructure.DbContexts;

namespace Shopping.Infrastructure.Repositories;

public class OrderRepository(ShoppingDbContextMaster dbContext)
    : GenericRepository<Order>(dbContext), IOrderRepository
{
    public async Task<Order?> GetByTransactionIdAsync(Guid transactionId,
        CancellationToken cancellationToken = default)
        => await dbContext.Orders
            .FirstOrDefaultAsync(o => o.TransactionId == transactionId, cancellationToken);
}
