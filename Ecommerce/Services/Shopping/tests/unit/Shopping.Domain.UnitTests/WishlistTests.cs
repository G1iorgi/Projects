using Shopping.Domain.Aggregates.WishlistAggregate;
using SharedKernel.Exceptions.Product;

namespace Shopping.Domain.UnitTests;

public class WishlistTests
{
    [Fact]
    public void Create_WithValidUserId_ShouldSucceed()
    {
        // Arrange
        const string userId = "user-123";

        // Act
        var wishlist = Wishlist.Create(userId);

        // Assert
        Assert.NotNull(wishlist);
        Assert.Equal(userId, wishlist.UserId);
        Assert.Empty(wishlist.WishlistItems);
    }

    [Fact]
    public void Create_WithNullUserId_ShouldThrow()
    {
        // Arrange
        string userId = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Wishlist.Create(userId));
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrow()
    {
        // Arrange
        const string userId = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Wishlist.Create(userId));
    }

    [Fact]
    public void Create_WithWhitespaceUserId_ShouldThrow()
    {
        // Arrange
        const string userId = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Wishlist.Create(userId));
    }

    [Fact]
    public void AddItem_WithValidProduct_ShouldAddItem()
    {
        // Arrange
        var wishlist = Wishlist.Create("user-123");
        const int productId = 100;
        const string productName = "Product A";
        const decimal price = 29.99m;

        // Act
        wishlist.AddItem(productId, productName, price);

        // Assert
        Assert.Single(wishlist.WishlistItems);

        var item = wishlist.WishlistItems.First();
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(productName, item.ProductName);
        Assert.Equal(price, item.ProductPrice);
    }

    [Fact]
    public void AddItem_WithMultipleDifferentProducts_ShouldAddAll()
    {
        // Arrange
        var wishlist = Wishlist.Create("user-123");

        // Act
        wishlist.AddItem(100, "Product A", 29.99m);
        wishlist.AddItem(101, "Product B", 49.99m);
        wishlist.AddItem(102, "Product C", 19.99m);

        // Assert
        Assert.Equal(3, wishlist.WishlistItems.Count);
        Assert.Contains(wishlist.WishlistItems, x => x.ProductId == 100);
        Assert.Contains(wishlist.WishlistItems, x => x.ProductId == 101);
        Assert.Contains(wishlist.WishlistItems, x => x.ProductId == 102);
    }

    [Fact]
    public void RemoveItem_WithExistingProduct_ShouldRemoveItem()
    {
        // Arrange
        var wishlist = Wishlist.Create("user-123");
        wishlist.AddItem(100, "Product A", 29.99m);
        wishlist.AddItem(101, "Product B", 49.99m);

        // Act
        wishlist.RemoveItem(100);

        // Assert
        Assert.Single(wishlist.WishlistItems);
        Assert.DoesNotContain(wishlist.WishlistItems, x => x.ProductId == 100);
        Assert.Contains(wishlist.WishlistItems, x => x.ProductId == 101);
    }

    [Fact]
    public void RemoveItem_WithNonExistingProduct_ShouldThrow()
    {
        // Arrange
        var wishlist = Wishlist.Create("user-123");
        wishlist.AddItem(100, "Product A", 29.99m);

        // Act & Assert
        Assert.Throws<ProductNotFoundException>(() => wishlist.RemoveItem(999));
    }

    [Fact]
    public void RemoveItem_FromEmptyWishlist_ShouldThrow()
    {
        // Arrange
        var wishlist = Wishlist.Create("user-123");

        // Act & Assert
        Assert.Throws<ProductNotFoundException>(() => wishlist.RemoveItem(100));
    }

    [Fact]
    public void WishlistItems_ShouldNotBeNull()
    {
        // Arrange
        var wishlist = Wishlist.Create("user-123");

        // Assert
        Assert.NotNull(wishlist.WishlistItems);
        Assert.Empty(wishlist.WishlistItems);
    }

    [Fact]
    public void AddSameProductMultipleTimes_ShouldAllowDuplicates()
    {
        // Arrange
        var wishlist = Wishlist.Create("user-123");

        // Act
        wishlist.AddItem(100, "Product A", 29.99m);
        wishlist.AddItem(100, "Product A", 29.99m);

        // Assert
        Assert.Equal(2, wishlist.WishlistItems.Count);
    }

    [Fact]
    public void Create_ShouldInitializeWithEmptyCollection()
    {
        // Arrange & Act
        var wishlist = Wishlist.Create("user-123");

        // Assert
        Assert.NotNull(wishlist.WishlistItems);
        Assert.Empty(wishlist.WishlistItems);
    }
}
