namespace Shopping.Domain.Aggregates.CartAggregate;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
