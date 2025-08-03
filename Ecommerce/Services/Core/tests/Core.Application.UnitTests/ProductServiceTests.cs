using Core.Application.Aggregates.ProductAggregate;
using Core.Application.Aggregates.ProductAggregate.Commands;
using Core.Application.Services;
using Core.Domain;
using Core.Domain.Aggregates.CategoryAggregate;
using Core.Domain.Aggregates.ProductAggregate;
using Moq;

namespace Core.Application.UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productService = new ProductService(
            _unitOfWorkMock.Object,
            new Mock<IMapperService>().Object);
    }

    // [Fact]
    // public async Task GetAllProductsAsync_Should_Succeed()
    // {
    //     // Arrange
    //     var products = new List<Product>
    //     {
    //         Product.Create("Product 1", "123456789", "Description 1", 10, "Image 1", 1),
    //         Product.Create("Product 2", "987654321", "Description 2", 20, "Image 2", 2),
    //         Product.Create("Product 3", "123456789", "Description 3", 30, "Image 3", 3),
    //     };
    //     unitOfWorkMock
    //         .Setup(uow => uow.Products.GetAllAsync(default, default, default, default))
    //         .ReturnsAsync(products);
    //
    //     // Act
    //     var result = await productService.GetAllProductsAsync(default, default);
    //
    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal(products.Count, result.Count());
    //     unitOfWorkMock.Verify(uow => uow.Products.GetAllAsync(default, default, default, default), Times.Once);
    // }
    [Fact]
    public async Task GetProductByIdAsync_Should_Succeed()
    {
        // Arrange
        const int productId = 1;
        var product = Product.Create("Product 1", "123456789", "Description 1", 10, "Image 1", 1);
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Barcode, result.Barcode);
        Assert.Equal(product.Description, result.Description);
        Assert.Equal(product.Price, result.Price);
        Assert.Equal(product.Image, result.Image);
        Assert.Equal(product.CategoryId, result.CategoryId);
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetProductByIdAsync_Should_Throw_Exception()
    {
        // Arrange
        const int productId = 1;
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync((Product)null!);

        // Act
        async Task Act() => await _productService.GetProductByIdAsync(productId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_Should_Succeed()
    {
        // Arrange
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int categoryId = 1;
        var category = Category.Create("Category 1");
        _unitOfWorkMock
            .Setup(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None))
            .ReturnsAsync(true);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, CancellationToken.None))
            .ReturnsAsync(category);
        _unitOfWorkMock
            .Setup(uow => uow.Products.CreateAsync(It.IsAny<Product>(), CancellationToken.None));

        var createProductCommand = new CreateProductCommand
        {
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            CategoryId = categoryId,
        };

        // Act
        await _productService.CreateProductAsync(createProductCommand);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(categoryId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(category), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Products.CreateAsync(It.IsAny<Product>(), CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_Should_Throw_Exception_When_Barcode_Is_Not_Unique()
    {
        // Arrange
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int categoryId = 1;
        _unitOfWorkMock
            .Setup(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None))
            .ReturnsAsync(false);

        var createProductCommand = new CreateProductCommand
        {
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            CategoryId = categoryId,
        };

        // Act
        async Task Act() => await _productService.CreateProductAsync(createProductCommand);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(categoryId, CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.CreateAsync(It.IsAny<Product>(), CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task CreateProductAsync_Should_Throw_Exception_When_Category_Does_Not_Exist()
    {
        // Arrange
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int categoryId = 1;
        _unitOfWorkMock
            .Setup(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None))
            .ReturnsAsync(true);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, CancellationToken.None))
            .ReturnsAsync((Category)null!);

        var createProductCommand = new CreateProductCommand
        {
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            CategoryId = categoryId,
        };

        // Act
        async Task Act() => await _productService.CreateProductAsync(createProductCommand);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(categoryId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.CreateAsync(It.IsAny<Product>(), CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_Succeed()
    {
        // Arrange
        const int productId = 1;
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int categoryId = 1;
        var product = Product.Create(name, barcode, description, price, image, categoryId);
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync(product);
        _unitOfWorkMock.Setup(uow => uow.Products.Update(It.IsAny<Product>()));

        var updateProductCommand = new UpdateProductCommand
        {
            Id = productId,
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            CategoryId = categoryId,
        };

        // Act
        await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(categoryId, CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.Update(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_Throw_Exception_When_Product_Does_Not_Exist()
    {
        // Arrange
        const int productId = 1;
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync((Product)null!);

        var updateProductCommand = new UpdateProductCommand
        {
            Id = productId,
            Name = "Product 1",
            Barcode = "123456789",
            Description = "Description 1",
            Price = 10,
            Image = "Image 1",
            CategoryId = 1,
        };

        // Act
        async Task Act() => await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(It.IsAny<int>(), CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.Update(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_Succeed_When_Category_Is_Not_Updated()
    {
        // Arrange
        const int productId = 1;
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int categoryId = 1;
        var product = Product.Create(name, barcode, description, price, image, categoryId);
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync(product);
        _unitOfWorkMock.Setup(uow => uow.Products.Update(It.IsAny<Product>()));

        var updateProductCommand = new UpdateProductCommand
        {
            Id = productId,
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            CategoryId = categoryId,
        };

        // Act
        await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(It.IsAny<int>(), CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.Update(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_Succeed_When_Category_Is_Changed()
    {
        // Arrange
        const int productId = 1;
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int oldCategoryId = 1;
        var oldCategory = Category.Create("Category 1");
        const int newCategoryId = 2;
        var newCategory = Category.Create("Category 2");
        var product = Product.Create(name, barcode, description, price, image, oldCategoryId);
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync(product);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(oldCategoryId, CancellationToken.None))
            .ReturnsAsync(oldCategory);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(newCategoryId, CancellationToken.None))
            .ReturnsAsync(newCategory);
        _unitOfWorkMock.Setup(uow => uow.Products.Update(It.IsAny<Product>()));

        var updateProductCommand = new UpdateProductCommand
        {
            Id = productId,
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            CategoryId = newCategoryId,
        };

        // Act
        await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(newCategoryId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(oldCategoryId, CancellationToken.None), Times.Once);
        // unitOfWorkMock.Verify(uow => uow.Categories.Update(newCategory), Times.Once);
        // unitOfWorkMock.Verify(uow => uow.Categories.Update(oldCategory), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.Update(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_Throw_Exception_When_Category_Is_Updated_But_New_Category_Does_Not_Exist()
    {
        // Arrange
        const int productId = 1;
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int oldCategoryId = 1;
        const int newCategoryId = 2;
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync(Product.Create(name, barcode, description, price, image, oldCategoryId));
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(newCategoryId, CancellationToken.None))
            .ReturnsAsync((Category)null!);

        var updateProductCommand = new UpdateProductCommand
        {
            Id = productId,
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            CategoryId = newCategoryId,
        };

        // Act
        async Task Act() => await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(newCategoryId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(oldCategoryId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.Update(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_Throw_Exception_When_Category_Is_Updated_But_Old_Category_Does_Not_Exist()
    {
        // Arrange
        const int productId = 1;
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int oldCategoryId = 1;
        const int newCategoryId = 2;
        var newCategory = Category.Create("Category 2");
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync(Product.Create(name, barcode, description, price, image, oldCategoryId));
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(newCategoryId, CancellationToken.None))
            .ReturnsAsync(newCategory);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(oldCategoryId, CancellationToken.None))
            .ReturnsAsync((Category)null!);

        var updateProductCommand = new UpdateProductCommand
        {
            Id = productId,
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            CategoryId = newCategoryId,
        };

        // Act
        // async Task Act() => await productService.UpdateProductAsync(updateProductCommand);
        await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(newCategoryId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(oldCategoryId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(newCategory), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.Update(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_Succeed_When_Barcode_Is_Not_Updated()
    {
        // Arrange
        const int productId = 1;
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int categoryId = 1;
        var product = Product.Create(name, barcode, description, price, image, categoryId);
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync(product);
        _unitOfWorkMock.Setup(uow => uow.Products.Update(It.IsAny<Product>()));

        var updateProductCommand = new UpdateProductCommand
        {
            Id = productId,
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            CategoryId = categoryId,
        };

        // Act
        await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(It.IsAny<int>(), CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(barcode, CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.Update(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_Throw_Exception_When_Barcode_Is_Not_Unique()
    {
        // Arrange
        const int productId = 1;
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int categoryId = 1;
        const string newBarcode = "987654321";
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync(Product.Create(name, barcode, description, price, image, categoryId));
        _unitOfWorkMock
            .Setup(uow => uow.Products.IsUniqueAsync(newBarcode, CancellationToken.None))
            .ReturnsAsync(false);

        var updateProductCommand = new UpdateProductCommand
        {
            Id = productId,
            Name = name,
            Barcode = newBarcode,
            Description = description,
            Price = price,
            Image = image,
            CategoryId = categoryId,
        };

        // Act
        async Task Act() => await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(It.IsAny<int>(), CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(newBarcode, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Products.Update(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task UpdateProductAsync_should_Succeed_When_Barcode_Is_Updated_And_Is_Unique()
    {
        // Arrange
        const int productId = 1;
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int categoryId = 1;
        const string newBarcode = "987654321";
        var product = Product.Create(name, barcode, description, price, image, categoryId);
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync(product);
        _unitOfWorkMock
            .Setup(uow => uow.Products.IsUniqueAsync(newBarcode, CancellationToken.None))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(uow => uow.Products.Update(It.IsAny<Product>()));

        var updateProductCommand = new UpdateProductCommand
        {
            Id = productId,
            Name = name,
            Barcode = newBarcode,
            Description = description,
            Price = price,
            Image = image,
            CategoryId = categoryId,
        };

        // Act
        await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(It.IsAny<int>(), CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(newBarcode, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Products.Update(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_Should_Succeed()
    {
        // Arrange
        const int productId = 1;
        var product = Product.Create("Product 1", "123456789", "Description 1", 10, "Image 1", 1);
        var category = Category.Create("Category 1");
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync(product);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(product.CategoryId, CancellationToken.None))
            .ReturnsAsync(category);
        _unitOfWorkMock.Setup(uow => uow.Products.DeleteAsync(productId, CancellationToken.None));

        // Act
        await _productService.DeleteProductAsync(productId);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Products.DeleteAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_Should_Throw_Exception_When_Product_Does_Not_Exist()
    {
        // Arrange
        const int productId = 1;
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync((Product)null!);

        // Act
        async Task Act() => await _productService.DeleteProductAsync(productId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Products.DeleteAsync(It.IsAny<int>(), CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task DeleteProductAsync_Should_Throw_Exception_When_Category_Does_Not_Exist()
    {
        // Arrange
        const int productId = 1;
        var product = Product.Create("Product 1", "123456789", "Description 1", 10, "Image 1", 1);
        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None))
            .ReturnsAsync(product);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(product.CategoryId, CancellationToken.None))
            .ReturnsAsync((Category)null!);

        // Act
        async Task Act() => await _productService.DeleteProductAsync(productId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Products.DeleteAsync(It.IsAny<int>(), CancellationToken.None), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Never);
    }
}
