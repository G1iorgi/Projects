using Core.Domain.Aggregates.IdentityAggregate;
using Core.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repositories;

public class RefreshTokenRepository(CoreDbContextMaster dbContext)
    : GenericRepository<RefreshToken>(dbContext), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
        => await CoreDbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

    public async Task<RefreshToken?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
        => await CoreDbContext.RefreshTokens
            .Where(
                rt =>
                    rt.UserId == userId &&
                    rt.Expiration > DateTime.UtcNow &&
                    !rt.IsRevoked)
            .FirstOrDefaultAsync(cancellationToken);
}
