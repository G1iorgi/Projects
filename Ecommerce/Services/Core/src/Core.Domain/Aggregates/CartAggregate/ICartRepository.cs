namespace Core.Domain.Aggregates.CartAggregate;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<IEnumerable<Cart>> GetCartItemsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Cart?> GetCartItemAsync(
        string userId,
        int productId,
        CancellationToken cancellationToken = default);
}
