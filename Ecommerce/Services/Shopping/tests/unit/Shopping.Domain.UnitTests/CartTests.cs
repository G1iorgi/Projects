using Shopping.Domain.Aggregates.CartAggregate;
using SharedKernel.Exceptions.Product;

namespace Shopping.Domain.UnitTests;

public class CartTests
{
    [Fact]
    public void Create_WithValidUserId_ShouldSucceed()
    {
        // Arrange
        const string userId = "user-123";

        // Act
        var cart = Cart.Create(userId);

        // Assert
        Assert.NotNull(cart);
        Assert.Equal(userId, cart.UserId);
        Assert.Empty(cart.CartItems);
        Assert.Equal(0m, cart.TotalPrice);
    }

    [Fact]
    public void Create_WithNullUserId_ShouldThrow()
    {
        // Arrange
        string userId = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Cart.Create(userId));
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrow()
    {
        // Arrange
        const string userId = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Cart.Create(userId));
    }

    [Fact]
    public void Create_WithWhitespaceUserId_ShouldThrow()
    {
        // Arrange
        const string userId = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Cart.Create(userId));
    }

    [Fact]
    public void AddItem_WithNewProduct_ShouldAddToCart()
    {
        // Arrange
        var cart = Cart.Create("user-123");
        const int productId = 100;
        const string productName = "Test Product";
        const int quantity = 5;
        const decimal price = 29.99m;

        // Act
        cart.AddItem(productId, productName, quantity, price);

        // Assert
        Assert.Single(cart.CartItems);
        var item = cart.CartItems.First();
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(productName, item.ProductName);
        Assert.Equal(quantity, item.ProductQuantity);
        Assert.Equal(price, item.ProductPrice);
    }

    [Fact]
    public void AddItem_WithExistingProduct_ShouldIncreaseQuantity()
    {
        // Arrange
        var cart = Cart.Create("user-123");
        const int productId = 100;
        const string productName = "Test Product";

        cart.AddItem(productId, productName, 5, 29.99m);

        // Act
        cart.AddItem(productId, productName, 3, 29.99m);

        // Assert
        Assert.Single(cart.CartItems);
        var item = cart.CartItems.First();
        Assert.Equal(8, item.ProductQuantity);
    }

    [Fact]
    public void AddItem_WithMultipleDifferentProducts_ShouldAddAll()
    {
        // Arrange
        var cart = Cart.Create("user-123");

        // Act
        cart.AddItem(100, "Product 1", 5, 29.99m);
        cart.AddItem(101, "Product 2", 3, 49.99m);
        cart.AddItem(102, "Product 3", 2, 19.99m);

        // Assert
        Assert.Equal(3, cart.CartItems.Count);
        Assert.Contains(cart.CartItems, item => item.ProductId == 100);
        Assert.Contains(cart.CartItems, item => item.ProductId == 101);
        Assert.Contains(cart.CartItems, item => item.ProductId == 102);
    }

    [Fact]
    public void RemoveItem_WithExistingProduct_ShouldRemove()
    {
        // Arrange
        var cart = Cart.Create("user-123");
        cart.AddItem(100, "Product 1", 5, 29.99m);
        cart.AddItem(101, "Product 2", 3, 49.99m);

        // Act
        cart.RemoveItem(100);

        // Assert
        Assert.Single(cart.CartItems);
        Assert.DoesNotContain(cart.CartItems, item => item.ProductId == 100);
        Assert.Contains(cart.CartItems, item => item.ProductId == 101);
    }

    [Fact]
    public void RemoveItem_WithNonExistingProduct_ShouldThrow()
    {
        // Arrange
        var cart = Cart.Create("user-123");
        cart.AddItem(100, "Product 1", 5, 29.99m);

        // Act & Assert
        Assert.Throws<ProductNotFoundException>(() => cart.RemoveItem(999));
    }

    [Fact]
    public void RemoveItem_FromEmptyCart_ShouldThrow()
    {
        // Arrange
        var cart = Cart.Create("user-123");

        // Act & Assert
        Assert.Throws<ProductNotFoundException>(() => cart.RemoveItem(100));
    }

    [Fact]
    public void TotalPrice_WithNoItems_ShouldReturnZero()
    {
        // Arrange
        var cart = Cart.Create("user-123");

        // Act
        var totalPrice = cart.TotalPrice;

        // Assert
        Assert.Equal(0m, totalPrice);
    }

    [Fact]
    public void TotalPrice_WithSingleItem_ShouldCalculateCorrectly()
    {
        // Arrange
        var cart = Cart.Create("user-123");
        cart.AddItem(100, "Product 1", 5, 29.99m);

        // Act
        var totalPrice = cart.TotalPrice;

        // Assert
        Assert.Equal(149.95m, totalPrice);
    }

    [Fact]
    public void TotalPrice_WithMultipleItems_ShouldCalculateCorrectly()
    {
        // Arrange
        var cart = Cart.Create("user-123");
        cart.AddItem(100, "Product 1", 5, 29.99m);    // 149.95
        cart.AddItem(101, "Product 2", 3, 49.99m);    // 149.97
        cart.AddItem(102, "Product 3", 2, 19.99m);    // 39.98

        // Act
        var totalPrice = cart.TotalPrice;

        // Assert
        Assert.Equal(339.90m, totalPrice);
    }

    [Fact]
    public void TotalPrice_AfterIncreasingQuantity_ShouldRecalculate()
    {
        // Arrange
        var cart = Cart.Create("user-123");
        cart.AddItem(100, "Product 1", 5, 29.99m);
        Assert.Equal(149.95m, cart.TotalPrice);

        // Act
        var item = cart.CartItems.First();
        item.IncreaseQuantity(5);

        // Assert
        Assert.Equal(299.90m, cart.TotalPrice);
    }

    [Fact]
    public void TotalPrice_AfterRemovingItem_ShouldRecalculate()
    {
        // Arrange
        var cart = Cart.Create("user-123");
        cart.AddItem(100, "Product 1", 5, 29.99m);    // 149.95
        cart.AddItem(101, "Product 2", 3, 49.99m);    // 149.97
        Assert.Equal(299.92m, cart.TotalPrice);

        // Act
        cart.RemoveItem(100);

        // Assert
        Assert.Equal(149.97m, cart.TotalPrice);
    }

    [Fact]
    public void CartItems_ShouldNotBeNull()
    {
        // Arrange & Act
        var cart = Cart.Create("user-123");

        // Assert
        Assert.NotNull(cart.CartItems);
        Assert.Empty(cart.CartItems);
    }

    [Fact]
    public void AddItem_SameProductMultipleTimes_ShouldOnlyHaveOneItemWithAccumulatedQuantity()
    {
        // Arrange
        var cart = Cart.Create("user-123");

        // Act
        cart.AddItem(100, "Product 1", 5, 29.99m);
        cart.AddItem(100, "Product 1", 3, 29.99m);
        cart.AddItem(100, "Product 1", 2, 29.99m);

        // Assert
        Assert.Single(cart.CartItems);
        var item = cart.CartItems.First();
        Assert.Equal(10, item.ProductQuantity);
        Assert.Equal(299.90m, cart.TotalPrice);
    }
}

