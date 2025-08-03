using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Aggregates.IdentityAggregate;

public class RefreshToken
{
    // Required for Entity Framework Core
    public RefreshToken()
    {
    }

    private RefreshToken(string token, string userId, DateTimeOffset expiration, DateTimeOffset createdAt)
    {
        Token = token;
        UserId = userId;
        Expiration = expiration;
        CreatedAt = createdAt;
    }

    public string Token { get; private set; }

    public DateTimeOffset Expiration { get; private set; }

    public string UserId { get; private set; }

    public bool IsRevoked { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    // TODO if we will remove this property, we should remove the Microsoft.Extensions.Identity.Stores from the Core.Domain project
    public virtual IdentityUser User { get; init; } = null!;

    public static RefreshToken Create(string token, string userId)
    {
        Guard.Against.NullOrWhiteSpace(token);
        Guard.Against.NullOrWhiteSpace(userId);

        // TODO get the expiration time from configuration
        return new RefreshToken(token, userId, DateTimeOffset.UtcNow.AddDays(7), DateTimeOffset.UtcNow);
    }

    // TODO this method is not used in the codebase, consider removing it
    // TODO consider removing property RevokedAt and IsRevoked if they are not used as well
    public void MarkAsRevoked()
    {
        IsRevoked = true;
        RevokedAt = DateTimeOffset.UtcNow;
    }
}
