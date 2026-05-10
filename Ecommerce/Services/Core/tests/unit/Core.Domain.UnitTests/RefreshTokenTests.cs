using Core.Domain.Aggregates.IdentityAggregate;

namespace Core.Domain.UnitTests;

public class RefreshTokenTests
{
    [Fact]
    public void Create_Should_Succed()
    {
        // Arrange
        const string token = "refresh_token_123";
        const string userId = "user_1";

        // Act
        var refreshToken = RefreshToken.Create(token, userId);

        // Assert
        Assert.NotNull(refreshToken);
        Assert.Equal(token, refreshToken.Token);
        Assert.Equal(userId, refreshToken.UserId);
        Assert.False(refreshToken.IsRevoked);
        Assert.True(refreshToken.CreatedAt <= DateTimeOffset.UtcNow);
        Assert.True(refreshToken.Expiration > DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Create_Should_ThrowArgumentNullException_WhenTokenIsNull()
    {
        const string token = null;
        const string userId = "user_1";

        Assert.Throws<ArgumentNullException>(() =>
            RefreshToken.Create(token!, userId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_ThrowArgumentException_WhenTokenIsInvalid(string token)
    {
        const string userId = "user_1";

        Assert.Throws<ArgumentException>(() =>
            RefreshToken.Create(token, userId));
    }

    [Fact]
    public void MarkAsRevoked_Should_SetIsRevokedTrue()
    {
        // Arrange
        var token = RefreshToken.Create("token", "user");

        // Act
        token.MarkAsRevoked();

        // Assert
        Assert.True(token.IsRevoked);
        Assert.NotNull(token.RevokedAt);
        Assert.True(token.RevokedAt <= DateTimeOffset.UtcNow);
    }
}
