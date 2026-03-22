using Shopping.Domain.Aggregates.OrderAggregate;
using Shopping.Infrastructure.DbContexts;

namespace Shopping.Infrastructure.Repositories;

public class OrderRepository(ShoppingDbContextMaster dbContext) : GenericRepository<Order>(dbContext), IOrderRepository;

