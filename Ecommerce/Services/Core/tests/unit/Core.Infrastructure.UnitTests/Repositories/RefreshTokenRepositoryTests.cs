using Core.Domain.Aggregates.IdentityAggregate;
using Core.Infrastructure.DbContexts;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.UnitTests.Repositories;

public class RefreshTokenRepositoryTests : IDisposable
{
    private readonly CoreDbContextMaster _dbContext;
    private readonly RefreshTokenRepository _repository;

    public RefreshTokenRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CoreDbContextMaster>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CoreDbContextMaster(options);
        _repository = new RefreshTokenRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task GetByTokenAsync_ShouldReturnNull_WhenTokenDoesNotExist()
    {
        // Arrange
        const string nonExistentToken = "NonExistentToken";

        // Act
        var result = await _repository.GetByTokenAsync(nonExistentToken, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTokenAsync_ShouldReturnRefreshToken_WhenTokenExists()
    {
        // Arrange
        const string existentToken = "ExistentToken";
        var token = RefreshToken.Create(
            existentToken,
            "UserId");
        await _dbContext.RefreshTokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTokenAsync(existentToken, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existentToken, result.Token);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnNull_WhenNoValidTokenExistsForUser()
    {
        // Arrange
        const string userId = "UserId";
        var token = RefreshToken.Create(
            "ExistentToken",
            userId);
        token.MarkAsRevoked();
        await _dbContext.RefreshTokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnRefreshToken_WhenValidTokenExistsForUser()
    {
        // Arrange
        const string userId = "UserId";
        var token = RefreshToken.Create(
            "ExistentToken",
            userId);
        await _dbContext.RefreshTokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.False(result.IsRevoked);
        Assert.True(result.Expiration > DateTime.UtcNow);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnNull_WhenNoTokenExistsForUser()
    {
        // Arrange
        const string userId = "NonExistentUserId";

        // Act
        var result = await _repository.GetByUserIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnNull_WhenTokenIsExpired()
    {
        // Arrange
        const string userId = "UserId";
        var token = RefreshToken.Create("ExpiredToken", userId);
        // Manually set expiration to past
        typeof(RefreshToken)
            .GetProperty(nameof(RefreshToken.Expiration))
            ?.SetValue(token, DateTimeOffset.UtcNow.AddDays(-1));
        await _dbContext.RefreshTokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTokenAsync_ShouldReturnToken_WithMultipleTokensInDatabase()
    {
        // Arrange
        const string targetToken = "TargetToken";
        var token1 = RefreshToken.Create("Token1", "UserId1");
        var token2 = RefreshToken.Create(targetToken, "UserId2");
        var token3 = RefreshToken.Create("Token3", "UserId3");
        await _dbContext.RefreshTokens.AddRangeAsync(token1, token2, token3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTokenAsync(targetToken, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetToken, result.Token);
        Assert.Equal("UserId2", result.UserId);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnFirstValidToken_WhenMultipleTokensExist()
    {
        // Arrange
        const string userId = "UserId";
        var token1 = RefreshToken.Create("Token1", userId);
        var token2 = RefreshToken.Create("Token2", userId);
        await _dbContext.RefreshTokens.AddRangeAsync(token1, token2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetByTokenAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetByTokenAsync("Token", cancellationToken));
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetByUserIdAsync("UserId", cancellationToken));
    }
}
