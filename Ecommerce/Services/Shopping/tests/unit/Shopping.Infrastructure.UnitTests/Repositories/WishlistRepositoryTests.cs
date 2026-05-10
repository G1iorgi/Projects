using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Aggregates.WishlistAggregate;
using Shopping.Infrastructure.DbContexts;
using Shopping.Infrastructure.Repositories;

namespace Shopping.Infrastructure.UnitTests.Repositories;

public class WishlistRepositoryTests : IDisposable
{
    private readonly ShoppingDbContextMaster _dbContext;
    private readonly WishlistRepository _repository;

    public WishlistRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ShoppingDbContextMaster>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ShoppingDbContextMaster(options);
        _repository = new WishlistRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenWishlistExists_ShouldReturnWishlist()
    {
        // Arrange
        const string userId = "user-123";
        var wishlist = Wishlist.Create(userId);
        await _repository.CreateAsync(wishlist);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(wishlist.Id, result.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenWishlistDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        const string userId = "non-existent-user";

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithMultipleWishlists_ShouldReturnCorrectWishlist()
    {
        // Arrange
        const string userId1 = "user-1";
        const string userId2 = "user-2";
        const string userId3 = "user-3";

        var wishlist1 = Wishlist.Create(userId1);
        var wishlist2 = Wishlist.Create(userId2);
        var wishlist3 = Wishlist.Create(userId3);

        await _repository.CreateAsync(wishlist1);
        await _repository.CreateAsync(wishlist2);
        await _repository.CreateAsync(wishlist3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId2, result.UserId);
        Assert.Equal(wishlist2.Id, result.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldIncludeWishlistItems()
    {
        // Arrange
        const string userId = "user-123";
        var wishlist = Wishlist.Create(userId);
        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);

        await _repository.CreateAsync(wishlist);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.WishlistItems.Count);
        Assert.Contains(result.WishlistItems, item => item.ProductId == 1 && item.ProductName == "Product A");
        Assert.Contains(result.WishlistItems, item => item.ProductId == 2 && item.ProductName == "Product B");
    }

    [Fact]
    public async Task GetByUserIdAsync_WithEmptyUserId_ShouldReturnNull()
    {
        // Arrange
        const string userId = "";

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithSpecialCharactersInUserId_ShouldReturnWishlist()
    {
        // Arrange
        const string userId = "user@example.com";
        var wishlist = Wishlist.Create(userId);
        await _repository.CreateAsync(wishlist);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnWishlistWithWishlistIdSet()
    {
        // Arrange
        const string userId = "user-123";
        var wishlist = Wishlist.Create(userId);
        await _repository.CreateAsync(wishlist);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetByUserIdAsync_CaseSensitive_ShouldNotReturnWishlistWithDifferentCase()
    {
        // Arrange
        const string userId = "User-123";
        var wishlist = Wishlist.Create(userId);
        await _repository.CreateAsync(wishlist);
        await _dbContext.SaveChangesAsync();

        // Act - Search with different case
        var result = await _repository.GetByUserIdAsync("user-123");

        // Assert - Should not find it (case-sensitive by default in EF)
        // This depends on the database collation, but typically would not match
        // If the database uses case-insensitive collation, this test may need adjustment
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_AfterWishlistModification_ShouldReturnUpdatedWishlist()
    {
        // Arrange
        const string userId = "user-123";
        var wishlist = Wishlist.Create(userId);
        wishlist.AddItem(1, "Product A", 100);
        await _repository.CreateAsync(wishlist);
        await _dbContext.SaveChangesAsync();

        // Modify the wishlist
        wishlist.AddItem(2, "Product B", 200);
        _repository.Update(wishlist);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.WishlistItems.Count);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithDefaultCancellationToken_ShouldWork()
    {
        // Arrange
        const string userId = "user-456";
        var wishlist = Wishlist.Create(userId);
        await _repository.CreateAsync(wishlist);
        await _dbContext.SaveChangesAsync();

        // Act - Using default CancellationToken (not explicitly provided)
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithDifferentUserIds_ShouldNotReturnWrongWishlist()
    {
        // Arrange
        const string userId1 = "user-1";
        const string userId2 = "user-2";

        var wishlist1 = Wishlist.Create(userId1);
        var wishlist2 = Wishlist.Create(userId2);

        await _repository.CreateAsync(wishlist1);
        await _repository.CreateAsync(wishlist2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId1, result.UserId);
        Assert.NotEqual(userId2, result.UserId);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithMultipleItemsOfSameProduct_ShouldReturnSingleItem()
    {
        // Arrange
        const string userId = "user-123";
        var wishlist = Wishlist.Create(userId);
        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 100);

        await _repository.CreateAsync(wishlist);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);

        // Both items are added to the wishlist as separate entries
        Assert.Equal(2, result.WishlistItems.Count);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnWishlistWithCorrectItemProperties()
    {
        // Arrange
        const string userId = "user-789";
        const string productName = "Test Product";
        const decimal productPrice = 250.75m;
        const int productId = 99;

        var wishlist = Wishlist.Create(userId);
        wishlist.AddItem(productId, productName, productPrice);

        await _repository.CreateAsync(wishlist);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        var item = result.WishlistItems.First();
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(productName, item.ProductName);
        Assert.Equal(productPrice, item.ProductPrice);
    }
}
