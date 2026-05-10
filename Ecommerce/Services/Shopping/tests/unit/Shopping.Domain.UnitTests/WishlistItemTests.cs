using Shopping.Domain.Aggregates.WishlistAggregate;

namespace Shopping.Domain.UnitTests;

public class WishlistItemTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        const int wishlistId = 1;
        const int productId = 100;
        const string productName = "Test Product";
        const decimal productPrice = 29.99m;

        // Act
        var wishlistItem = WishlistItem.Create(wishlistId, productId, productName, productPrice);

        // Assert
        Assert.NotNull(wishlistItem);
        Assert.Equal(wishlistId, wishlistItem.WishlistId);
        Assert.Equal(productId, wishlistItem.ProductId);
        Assert.Equal(productName, wishlistItem.ProductName);
        Assert.Equal(productPrice, wishlistItem.ProductPrice);
    }

    [Fact]
    public void Create_WithNullProductName_ShouldThrow()
    {
        // Arrange
        const int wishlistId = 1;
        const int productId = 100;
        string productName = null!;
        const decimal productPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            WishlistItem.Create(wishlistId, productId, productName, productPrice));
    }

    [Fact]
    public void Create_WithEmptyProductName_ShouldThrow()
    {
        // Arrange
        const int wishlistId = 1;
        const int productId = 100;
        const string productName = "";
        const decimal productPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            WishlistItem.Create(wishlistId, productId, productName, productPrice));
    }

    [Fact]
    public void Create_WithWhitespaceProductName_ShouldThrow()
    {
        // Arrange
        const int wishlistId = 1;
        const int productId = 100;
        const string productName = "   ";
        const decimal productPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            WishlistItem.Create(wishlistId, productId, productName, productPrice));
    }

    [Fact]
    public void Create_WithZeroPrice_ShouldThrow()
    {
        // Arrange
        const int wishlistId = 1;
        const int productId = 100;
        const string productName = "Test Product";
        const decimal productPrice = 0m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            WishlistItem.Create(wishlistId, productId, productName, productPrice));
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrow()
    {
        // Arrange
        const int wishlistId = 1;
        const int productId = 100;
        const string productName = "Test Product";
        const decimal productPrice = -29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            WishlistItem.Create(wishlistId, productId, productName, productPrice));
    }

    [Fact]
    public void Create_WithSmallDecimalPrice_ShouldSucceed()
    {
        // Arrange
        const int wishlistId = 1;
        const int productId = 100;
        const string productName = "Test Product";
        const decimal productPrice = 0.01m;

        // Act
        var wishlistItem = WishlistItem.Create(wishlistId, productId, productName, productPrice);

        // Assert
        Assert.NotNull(wishlistItem);
        Assert.Equal(productPrice, wishlistItem.ProductPrice);
    }

    [Fact]
    public void Create_WithLargeDecimalPrice_ShouldSucceed()
    {
        // Arrange
        const int wishlistId = 1;
        const int productId = 100;
        const string productName = "Test Product";
        const decimal productPrice = 999999.99m;

        // Act
        var wishlistItem = WishlistItem.Create(wishlistId, productId, productName, productPrice);

        // Assert
        Assert.NotNull(wishlistItem);
        Assert.Equal(productPrice, wishlistItem.ProductPrice);
    }

    [Fact]
    public void Create_WithSpecialCharactersInProductName_ShouldSucceed()
    {
        // Arrange
        const int wishlistId = 1;
        const int productId = 100;
        const string productName = "Test Product @#$% - 123";
        const decimal productPrice = 29.99m;

        // Act
        var wishlistItem = WishlistItem.Create(wishlistId, productId, productName, productPrice);

        // Assert
        Assert.NotNull(wishlistItem);
        Assert.Equal(productName, wishlistItem.ProductName);
    }

    [Fact]
    public void Create_WithLongProductName_ShouldSucceed()
    {
        // Arrange
        const int wishlistId = 1;
        const int productId = 100;
        var productName = new string('A', 500);
        const decimal productPrice = 29.99m;

        // Act
        var wishlistItem = WishlistItem.Create(wishlistId, productId, productName, productPrice);

        // Assert
        Assert.NotNull(wishlistItem);
        Assert.Equal(productName, wishlistItem.ProductName);
    }

    [Fact]
    public void Create_WithZeroWishlistId_ShouldSucceed()
    {
        // Arrange
        const int wishlistId = 0;
        const int productId = 100;
        const string productName = "Test Product";
        const decimal productPrice = 29.99m;

        // Act
        var wishlistItem = WishlistItem.Create(wishlistId, productId, productName, productPrice);

        // Assert
        Assert.NotNull(wishlistItem);
        Assert.Equal(wishlistId, wishlistItem.WishlistId);
    }

    [Fact]
    public void Create_WithZeroProductId_ShouldSucceed()
    {
        // Arrange
        const int wishlistId = 1;
        const int productId = 0;
        const string productName = "Test Product";
        const decimal productPrice = 29.99m;

        // Act
        var wishlistItem = WishlistItem.Create(wishlistId, productId, productName, productPrice);

        // Assert
        Assert.NotNull(wishlistItem);
        Assert.Equal(productId, wishlistItem.ProductId);
    }

    [Fact]
    public void WishlistItem_PropertiesAreReadOnly_AfterCreation()
    {
        // Arrange
        var wishlistItem = WishlistItem.Create(1, 100, "Test Product", 29.99m);

        // Act & Assert - Verify properties can be read
        Assert.Equal(1, wishlistItem.WishlistId);
        Assert.Equal(100, wishlistItem.ProductId);
        Assert.Equal("Test Product", wishlistItem.ProductName);
        Assert.Equal(29.99m, wishlistItem.ProductPrice);
    }
}

