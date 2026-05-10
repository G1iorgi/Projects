using Shopping.Domain.Aggregates.CartAggregate;

namespace Shopping.Domain.UnitTests;

public class CartItemTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        const int cartId = 1;
        const int productId = 100;
        const string productName = "Test Product";
        const int productQuantity = 5;
        const decimal productPrice = 29.99m;

        // Act
        var cartItem = CartItem.Create(cartId, productId, productName, productQuantity, productPrice);

        // Assert
        Assert.NotNull(cartItem);
        Assert.Equal(cartId, cartItem.CartId);
        Assert.Equal(productId, cartItem.ProductId);
        Assert.Equal(productName, cartItem.ProductName);
        Assert.Equal(productQuantity, cartItem.ProductQuantity);
        Assert.Equal(productPrice, cartItem.ProductPrice);
    }

    [Fact]
    public void Create_WithNullProductName_ShouldThrow()
    {
        // Arrange
        const int cartId = 1;
        const int productId = 100;
        string productName = null!;
        const int productQuantity = 5;
        const decimal productPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            CartItem.Create(cartId, productId, productName, productQuantity, productPrice));
    }

    [Fact]
    public void Create_WithEmptyProductName_ShouldThrow()
    {
        // Arrange
        const int cartId = 1;
        const int productId = 100;
        const string productName = "";
        const int productQuantity = 5;
        const decimal productPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            CartItem.Create(cartId, productId, productName, productQuantity, productPrice));
    }

    [Fact]
    public void Create_WithWhitespaceProductName_ShouldThrow()
    {
        // Arrange
        const int cartId = 1;
        const int productId = 100;
        const string productName = "   ";
        const int productQuantity = 5;
        const decimal productPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            CartItem.Create(cartId, productId, productName, productQuantity, productPrice));
    }

    [Fact]
    public void Create_WithZeroQuantity_ShouldThrow()
    {
        // Arrange
        const int cartId = 1;
        const int productId = 100;
        const string productName = "Test Product";
        const int productQuantity = 0;
        const decimal productPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            CartItem.Create(cartId, productId, productName, productQuantity, productPrice));
    }

    [Fact]
    public void Create_WithNegativeQuantity_ShouldThrow()
    {
        // Arrange
        const int cartId = 1;
        const int productId = 100;
        const string productName = "Test Product";
        const int productQuantity = -5;
        const decimal productPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            CartItem.Create(cartId, productId, productName, productQuantity, productPrice));
    }

    [Fact]
    public void Create_WithZeroPrice_ShouldThrow()
    {
        // Arrange
        const int cartId = 1;
        const int productId = 100;
        const string productName = "Test Product";
        const int productQuantity = 5;
        const decimal productPrice = 0m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            CartItem.Create(cartId, productId, productName, productQuantity, productPrice));
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrow()
    {
        // Arrange
        const int cartId = 1;
        const int productId = 100;
        const string productName = "Test Product";
        const int productQuantity = 5;
        const decimal productPrice = -29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            CartItem.Create(cartId, productId, productName, productQuantity, productPrice));
    }

    [Fact]
    public void IncreaseQuantity_WithValidQuantity_ShouldAddToCurrentQuantity()
    {
        // Arrange
        var cartItem = CartItem.Create(1, 100, "Test Product", 5, 29.99m);
        const int additionalQuantity = 3;

        // Act
        cartItem.IncreaseQuantity(additionalQuantity);

        // Assert
        Assert.Equal(8, cartItem.ProductQuantity);
    }

    [Fact]
    public void IncreaseQuantity_WithZeroQuantity_ShouldThrow()
    {
        // Arrange
        var cartItem = CartItem.Create(1, 100, "Test Product", 5, 29.99m);
        const int additionalQuantity = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => cartItem.IncreaseQuantity(additionalQuantity));
    }

    [Fact]
    public void IncreaseQuantity_WithNegativeQuantity_ShouldThrow()
    {
        // Arrange
        var cartItem = CartItem.Create(1, 100, "Test Product", 5, 29.99m);
        const int additionalQuantity = -3;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => cartItem.IncreaseQuantity(additionalQuantity));
    }

    [Fact]
    public void IncreaseQuantity_MultipleInvocations_ShouldAccumulate()
    {
        // Arrange
        var cartItem = CartItem.Create(1, 100, "Test Product", 5, 29.99m);

        // Act
        cartItem.IncreaseQuantity(2);
        cartItem.IncreaseQuantity(3);
        cartItem.IncreaseQuantity(1);

        // Assert
        Assert.Equal(11, cartItem.ProductQuantity);
    }
}

