using Core.Domain.Aggregates.ProductAggregate;

namespace Core.Domain.UnitTests;

public class ProductTests
{
    [Fact]
    public void Create_Should_Succeed()
    {
        // Arrange
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description";
        const decimal price = 10.5m;
        const string image = "image_url";
        const int quantity = 100;
        const int categoryId = 1;

        // Act
        var product = Product.Create(name: name,
            barcode: barcode,
            description: description,
            price: price,
            image: image,
            quantity: quantity,
            categoryId: categoryId);

        // Assert
        Assert.NotNull(product);
        Assert.Equal(name, product.Name);
        Assert.Equal(barcode, product.Barcode);
        Assert.Equal(description, product.Description);
        Assert.Equal(price, product.Price);
        Assert.Equal(image, product.Image);
        Assert.Equal(quantity, product.Quantity);
        Assert.Equal(categoryId, product.CategoryId);
        Assert.Equal(ProductStatus.Enabled, product.Status);
        Assert.True(product.CreateDate <= DateTimeOffset.UtcNow);
    }

    [Theory]
    [InlineData(null, "123456789", 10.5, 1, 1)]
    [InlineData("", "123456789", 10.5, 1, 1)]
    [InlineData("Product 1", null, 10.5, 1, 1)]
    [InlineData("Product 1", "", 10.5, 1, 1)]
    [InlineData("Product 1", "123456789", 0, 1, 1)]
    [InlineData("Product 1", "123456789", -1, 1, 1)]
    [InlineData("Product 1", "123456789", 10.5, 1, 0)]
    [InlineData("Product 1", "123456789", 10.5, 1, -1)]
    public void Create_Should_ThrowExceptionOnInvalidData(string name, string barcode, decimal price, int quantity, int categoryId)
    {
        // Arrange & Act
        var action = () => Product.Create(name, barcode, null, price, null, quantity, categoryId);

        // Assert
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(barcode))
        {
            if (name == null || barcode == null)
            {
                Assert.Throws<ArgumentNullException>(action);
            }
            else
            {
                Assert.Throws<ArgumentException>(action);
            }
        }
        else if (price <= 0 || categoryId <= 0)
        {
            Assert.Throws<ArgumentException>(action);
        }
    }

    [Fact]
    public void UpdateMetadata_Should_Succeed()
    {
        // Arrange
        var product = Product.Create("Product 1", "123456789", null, 10.5m, null, 1, 1);
        var productId = product.Id;
        const string name = "Product 2";
        const string barcode = "987654321";
        const decimal price = 20.5m;
        const int quantity = 1;
        const int categoryId = 2;

        // Act
        product.UpdateMetadata(name, barcode, null, price, null, quantity, categoryId);

        // Assert
        Assert.Equal(productId, product.Id);
        Assert.Equal(name, product.Name);
        Assert.Equal(barcode, product.Barcode);
        Assert.Equal(price, product.Price);
        Assert.Equal(ProductStatus.Enabled, product.Status);
        Assert.Equal(categoryId, product.CategoryId);
    }

    [Theory]
    [InlineData(null, "123456789", 10.5, 1, 1)]
    [InlineData("Product 1", null, 10.5, 1, 1)]
    [InlineData("Product 1", "123456789", 0, 1, 1)]
    [InlineData("Product 1", "123456789", 10.5, 1, 0)]
    public void UpdateMetadata_Should_ThrowExceptionOnInvalidData(string name, string barcode, decimal price, int quantity,
        int categoryId)
    {
        // Arrange
        var product = Product.Create("Product 1", "123456789", null, 10.5m, null, 1, 1);

        // Act
        var action = () => product.UpdateMetadata(name, barcode, null, price, null, quantity, categoryId);

        // Assert
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(barcode))
        {
            Assert.Throws<ArgumentNullException>(action);
        }
        else if (price <= 0 || categoryId <= 0)
        {
            Assert.Throws<ArgumentException>(action);
        }
    }

    [Fact]
    public void BarcodeHasChanged_Should_WorkCorrectly()
    {
        // Arrange
        const string name = "Product 1";
        const string barcode = "123456789";
        const decimal price = 10.5m;
        const int quantity = 1;
        const int categoryId = 1;

        // Act
        var product = Product.Create(name,
            barcode,
            null,
            price,
            null,
            quantity,
            categoryId);

        // Assert
        Assert.False(product.BarcodeHasChanged(barcode));
        Assert.True(product.BarcodeHasChanged("987654321"));
    }

    [Fact]
    public void CategoryHasChanged_Should_WorkCorrectly()
    {
        // Arrange
        const string name = "Product 1";
        const string barcode = "123456789";
        const decimal price = 10.5m;
        const int quantity = 1;
        const int categoryId = 1;

        // Act
        var product = Product.Create(name,
            barcode,
            null,
            price,
            null,
            quantity,
            categoryId);

        // Assert
        Assert.False(product.CategoryHasChanged(categoryId));
        Assert.True(product.CategoryHasChanged(2));
    }

    [Fact]
    public void HasEnoughQuantity_Should_WorkCorrectly()
    {
        // Arrange
        const string name = "Product 1";
        const string barcode = "123456789";
        const decimal price = 10.5m;
        const int quantity = 10;
        const int categoryId = 1;

        // Act
        var product = Product.Create(name,
            barcode,
            null,
            price,
            null,
            quantity,
            categoryId);

        // Assert
        Assert.True(product.HasEnoughQuantity(5));
        Assert.False(product.HasEnoughQuantity(11));
    }

    [Fact]
    public void DecreaseQuantity_Should_WorkCorrectly()
    {
        // Arrange
        const string name = "Product 1";
        const string barcode = "123456789";
        const decimal price = 10.5m;
        const int quantity = 10;
        const int categoryId = 1;

        // Act
        var product = Product.Create(name,
            barcode,
            null,
            price,
            null,
            quantity,
            categoryId);

        product.DecreaseQuantity(5);

        // Assert
        Assert.Equal(5, product.Quantity);
    }

    [Fact]
    public void DecreaseQuantity_Should_ThrowExceptionOnInvalidData()
    {
        // Arrange
        const string name = "Product 1";
        const string barcode = "123456789";
        const decimal price = 10.5m;
        const int quantity = 10;
        const int categoryId = 1;

        // Act
        var product = Product.Create(name,
            barcode,
            null,
            price,
            null,
            quantity,
            categoryId);

        // Assert
        Assert.Throws<ArgumentException>(() =>
            product.DecreaseQuantity(0));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void DecreaseQuantity_Should_ThrowExceptionOnNegativeOrZeroQuantity(int decreaseQuantity)
    {
        // Arrange
        const string name = "Product 1";
        const string barcode = "123456789";
        const decimal price = 10.5m;
        const int quantity = 10;
        const int categoryId = 1;

        // Act
        var product = Product.Create(name,
            barcode,
            null,
            price,
            null,
            quantity,
            categoryId);

        // Assert
        Assert.Throws<ArgumentException>(() =>
            product.DecreaseQuantity(decreaseQuantity));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void HasEnoughQuantity_Should_ThrowExceptionOnNegativeOrZeroQuantity(int checkQuantity)
    {
        // Arrange
        const string name = "Product 1";
        const string barcode = "123456789";
        const decimal price = 10.5m;
        const int quantity = 10;
        const int categoryId = 1;

        // Act
        var product = Product.Create(name,
            barcode,
            null,
            price,
            null,
            quantity,
            categoryId);

        // Assert
        Assert.Throws<ArgumentException>(() =>
            product.HasEnoughQuantity(checkQuantity));
    }
}
