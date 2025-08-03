namespace Core.Domain.Aggregates.IdentityAggregate;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
