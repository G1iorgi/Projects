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
        const decimal price = 10.5m;
        const int categoryId = 1;

        // Act
        var product = Product.Create(name, barcode, null, price, null, categoryId);

        // Assert
        Assert.NotNull(product);
        Assert.Equal(name, product.Name);
        Assert.Equal(barcode, product.Barcode);
        Assert.Equal(price, product.Price);
        Assert.Equal(ProductStatus.Enabled, product.Status);
        Assert.Equal(categoryId, product.CategoryId);
    }

    [Theory]
    [InlineData(null, "123456789", 10.5, 1)]
    [InlineData("", "123456789", 10.5, 1)]
    [InlineData("Product 1", null, 10.5, 1)]
    [InlineData("Product 1", "", 10.5, 1)]
    [InlineData("Product 1", "123456789", 0, 1)]
    [InlineData("Product 1", "123456789", -1, 1)]
    [InlineData("Product 1", "123456789", 10.5, 0)]
    [InlineData("Product 1", "123456789", 10.5, -1)]
    public void Create_Should_ThrowExpectionOnInvalidData(string name, string barcode, decimal price, int categoryId)
    {
        // Arrange & Act
        var action = () => Product.Create(name, barcode, null, price, null, categoryId);

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
        var product = Product.Create("Product 1", "123456789", null, 10.5m, null, 1);
        var productId = product.Id;
        const string name = "Product 2";
        const string barcode = "987654321";
        const decimal price = 20.5m;
        const int categoryId = 2;

        // Act
        product.UpdateMetadata(name, barcode, null, price, null, categoryId);

        // Assert
        Assert.Equal(productId, product.Id);
        Assert.Equal(name, product.Name);
        Assert.Equal(barcode, product.Barcode);
        Assert.Equal(price, product.Price);
        Assert.Equal(ProductStatus.Enabled, product.Status);
        Assert.Equal(categoryId, product.CategoryId);
    }

    [Theory]
    [InlineData(null, "123456789", 10.5, 1)]
    [InlineData("Product 1", null, 10.5, 1)]
    [InlineData("Product 1", "123456789", 0, 1)]
    [InlineData("Product 1", "123456789", 10.5, 0)]
    public void UpdateMetadata_Should_ThrowExpectionOnInvalidData(string name, string barcode, decimal price,
        int categoryId)
    {
        // Arrange
        var product = Product.Create("Product 1", "123456789", null, 10.5m, null, 1);

        // Act
        var action = () => product.UpdateMetadata(name, barcode, null, price, null, categoryId);

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
}
