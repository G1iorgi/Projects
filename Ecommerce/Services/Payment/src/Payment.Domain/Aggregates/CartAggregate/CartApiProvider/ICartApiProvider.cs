using Payment.Domain.Aggregates.CartAggregate.CartApiProvider.DTOs;

namespace Payment.Domain.Aggregates.CartAggregate.CartApiProvider;

public interface ICartApiProvider
{
    Task<List<CartItem>?> GetCartItemsByUserId(string jwt, CancellationToken cancellationToken = default);
}
