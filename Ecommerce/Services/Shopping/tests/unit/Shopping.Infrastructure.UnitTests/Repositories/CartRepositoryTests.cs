using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Aggregates.CartAggregate;
using Shopping.Infrastructure.DbContexts;
using Shopping.Infrastructure.Repositories;

namespace Shopping.Infrastructure.UnitTests.Repositories;

public class CartRepositoryTests : IDisposable
{
    private readonly ShoppingDbContextMaster _dbContext;
    private readonly CartRepository _cartRepository;

    public CartRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ShoppingDbContextMaster>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ShoppingDbContextMaster(options);
        _cartRepository = new CartRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenCartExists_ShouldReturnCart()
    {
        // Arrange
        const string userId = "user-123";
        var cart = Cart.Create(userId);
        await _cartRepository.CreateAsync(cart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cartRepository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(cart.Id, result.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenCartDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        const string userId = "non-existent-user";

        // Act
        var result = await _cartRepository.GetByUserIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithMultipleCartsInDatabase_ShouldReturnCorrectCart()
    {
        // Arrange
        const string userId1 = "user-1";
        const string userId2 = "user-2";
        const string userId3 = "user-3";

        var cart1 = Cart.Create(userId1);
        var cart2 = Cart.Create(userId2);
        var cart3 = Cart.Create(userId3);

        await _cartRepository.CreateAsync(cart1);
        await _cartRepository.CreateAsync(cart2);
        await _cartRepository.CreateAsync(cart3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cartRepository.GetByUserIdAsync(userId2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId2, result.UserId);
        Assert.Equal(cart2.Id, result.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldIncludeCartItems()
    {
        // Arrange
        const string userId = "user-123";
        var cart = Cart.Create(userId);
        cart.AddItem(1, "Product A", 2, 100);
        cart.AddItem(2, "Product B", 1, 200);

        await _cartRepository.CreateAsync(cart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cartRepository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.CartItems.Count);
        Assert.Contains(result.CartItems, item => item.ProductId == 1 && item.ProductName == "Product A");
        Assert.Contains(result.CartItems, item => item.ProductId == 2 && item.ProductName == "Product B");
    }

    [Fact]
    public async Task GetByUserIdAsync_WithEmptyUserId_ShouldReturnNull()
    {
        // Arrange
        const string userId = "";

        // Act
        var result = await _cartRepository.GetByUserIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnCartWithCorrectTotalPrice()
    {
        // Arrange
        const string userId = "user-123";
        var cart = Cart.Create(userId);
        cart.AddItem(1, "Product A", 2, 100); // 2 * 100 = 200
        cart.AddItem(2, "Product B", 3, 50);  // 3 * 50 = 150

        await _cartRepository.CreateAsync(cart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cartRepository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(350, result.TotalPrice);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithSpecialCharactersInUserId_ShouldReturnCart()
    {
        // Arrange
        const string userId = "user@example.com";
        var cart = Cart.Create(userId);
        await _cartRepository.CreateAsync(cart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cartRepository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnCartWithCartIdSet()
    {
        // Arrange
        const string userId = "user-123";
        var cart = Cart.Create(userId);
        await _cartRepository.CreateAsync(cart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cartRepository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetByUserIdAsync_CaseSensitive_ShouldNotReturnCartWithDifferentCase()
    {
        // Arrange
        const string userId = "User-123";
        var cart = Cart.Create(userId);
        await _cartRepository.CreateAsync(cart);
        await _dbContext.SaveChangesAsync();

        // Act - Search with different case
        var result = await _cartRepository.GetByUserIdAsync("user-123");

        // Assert - Should not find it (case-sensitive by default in EF)
        // This depends on the database collation, but typically would not match
        // If the database uses case-insensitive collation, this test may need adjustment
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_AfterCartModification_ShouldReturnUpdatedCart()
    {
        // Arrange
        const string userId = "user-123";
        var cart = Cart.Create(userId);
        cart.AddItem(1, "Product A", 1, 100);
        await _cartRepository.CreateAsync(cart);
        await _dbContext.SaveChangesAsync();

        // Modify the cart
        cart.AddItem(2, "Product B", 2, 50);
        _cartRepository.Update(cart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cartRepository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.CartItems.Count);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldUseFirstOrDefault()
    {
        // This test verifies the behavior when using FirstOrDefaultAsync
        // There should only be one cart per user, so FirstOrDefault is safe

        // Arrange
        const string userId = "user-123";
        var cart = Cart.Create(userId);
        await _cartRepository.CreateAsync(cart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cartRepository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(await _dbContext.Carts.Where(c => c.UserId == userId).ToListAsync());
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNullUserId_ShouldReturnNull()
    {
        // Act & Assert
        var result = await _cartRepository.GetByUserIdAsync(null!);

        // This should either throw or return null depending on implementation
        // If it doesn't throw, it should return null
        Assert.Null(result);
    }
}

