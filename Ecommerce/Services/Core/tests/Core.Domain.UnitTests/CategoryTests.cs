using Core.Domain.Aggregates.CategoryAggregate;
using Core.Domain.Aggregates.ProductAggregate;

namespace Core.Domain.UnitTests;

public class CategoryTests
{
    [Fact]
    public void Create_Should_Succeed()
    {
        // Arrange
        const string name = "Category 1";

        // Act
        var category = Category.Create(name);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(name, category.Name);
        Assert.Equal(CategoryStatus.Enabled, category.Status);
        Assert.Equal(category.Products.Count, category.ProductQuantity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_Should_ThrowExpectionOnInvalidData(string name)
    {
        // Arrange & Act
        var action = () => Category.Create(name);

        // Assert
        if (name == null)
        {
            Assert.Throws<ArgumentNullException>(action);
        }
        else if (string.IsNullOrWhiteSpace(name))
        {
            Assert.Throws<ArgumentException>(action);
        }
    }

    [Fact]
    public void UpdateMetadata_Should_Succeed()
    {
        // Arrange
        var category = Category.Create("Category 1");
        var categoryId = category.Id;
        const string name = "Category 2";

        // Act
        category.UpdateMetadata(name);

        // Assert
        Assert.Equal(categoryId, category.Id);
        Assert.Equal(name, category.Name);
        Assert.Equal(CategoryStatus.Enabled, category.Status);
        Assert.Equal(category.Products.Count, category.ProductQuantity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void UpdateMetadata_Should_ThrowExpectionOnInvalidData(string name)
    {
        // Arrange
        var category = Category.Create("Category 1");

        // Act
        var action = () => category.UpdateMetadata(name);

        // Assert
        if (name == null)
        {
            Assert.Throws<ArgumentNullException>(action);
        }
        else if (string.IsNullOrWhiteSpace(name))
        {
            Assert.Throws<ArgumentException>(action);
        }
    }

    [Fact]
    public void AddProduct_Should_Succeed()
    {
        // Arrange
        var category = Category.Create("Category 1");
        var product = Product.Create("Product 1", "123456789", "Description 1", 10, "Image 1", 1);
        var productQuantity = category.ProductQuantity;

        // Act
        category.AddProduct(product);

        // Assert
        Assert.Equal(productQuantity + 1, category.ProductQuantity);
    }

    [Fact]
    public void AddProduct_Should_ThrowExpectionOnInvalidData()
    {
        // Arrange
        var category = Category.Create("Category 1");

        // Act
        var action = () => category.AddProduct(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void RemoveProduct_Should_Succeed()
    {
        // Arrange
        var category = Category.Create("Category 1");
        var product = Product.Create("Product 1", "123456789", "Description 1", 10, "Image 1", 1);
        category.AddProduct(product);
        var productQuantity = category.ProductQuantity;

        // Act
        category.RemoveProduct(product);

        // Assert
        Assert.Equal(productQuantity - 1, category.ProductQuantity);
    }

    [Fact]
    public void RemoveProduct_Should_ReturnFalse()
    {
        // Arrange
        var category = Category.Create("Category 1");
        var product = Product.Create("Product 1", "123456789", "Description 1", 10, "Image 1", 1);

        // Act
        var result = category.RemoveProduct(product);

        // Assert
        Assert.False(result);
    }
}
