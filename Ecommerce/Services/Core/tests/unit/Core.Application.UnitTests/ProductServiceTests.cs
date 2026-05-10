using Core.Application.Aggregates.ProductAggregate;
using Core.Application.Aggregates.ProductAggregate.Commands;
using Core.Application.Aggregates.ProductAggregate.DTOs;
using Core.Application.Services;
using Core.Domain;
using Core.Domain.Aggregates.CategoryAggregate;
using Core.Domain.Aggregates.ProductAggregate;
using Moq;
using SharedKernel.Exceptions.Category;
using SharedKernel.Exceptions.Product;

namespace Core.Application.UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapperService> _mapperMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapperService>();
        _productService = new ProductService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    private static IQueryable<Product> GetProductQuery()
    {
        return new List<Product>
        {
            Product.Create("Phone", "111", "Good phone", 500, "img", 10, 1),
            Product.Create("Laptop", "222", "Good laptop", 1500, "img", 5, 1),
            Product.Create("Mouse", "333", "Small mouse", 20, null, 50, 1)
        }.AsQueryable();
    }

    [Fact]
    public async Task GetAllProducts_Should_Return_All_Data()
    {
        var query = GetProductQuery();

        _unitOfWorkMock.Setup(x => x.Products.GetAll())
            .Returns(query);

        _unitOfWorkMock.Setup(x => x.Products.ToPagedList(
                It.IsAny<IQueryable<Product>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(query.ToList());

        _mapperMock.Setup(x => x.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
            .Returns(new List<ProductDTO>
            {
                new(1, "Phone", "111", "Good phone", 500, "img", 10, DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "Category"),
                new(2, "Laptop", "222", "Good laptop", 1500, "img", 5, DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "Category"),
                new(3, "Mouse", "333", "Small mouse", 20, null, 50, DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "Category")
            });

        var result = await _productService.GetAllProductsAsync(10, 1);

        Assert.Equal(3, result.Count());

        _unitOfWorkMock.Verify(x => x.Products.GetAll(), Times.Once);
        _unitOfWorkMock.Verify(x => x.Products.ToPagedList(
            It.IsAny<IQueryable<Product>>(), 10, 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllProducts_Should_Filter_By_Name()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("Phone", "111", "desc", 10, "img", 1, 1),
            Product.Create("Laptop", "222", "desc", 10, "img", 1, 1),
            Product.Create("Mouse", "333", "desc", 10, "img", 1, 1),
        };

        _unitOfWorkMock.Setup(x => x.Products.GetAll())
            .Returns(products.AsQueryable());

        // IMPORTANT: DO NOT simulate paging logic
        _unitOfWorkMock.Setup(x => x.Products.ToPagedList(
                It.IsAny<IQueryable<Product>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<Product> q, int pageSize, int pageNumber, CancellationToken ct)
                => q.ToList());

        _mapperMock.Setup(x => x.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
            .Returns((IEnumerable<Product> products) => products.Select(p =>
                new ProductDTO(
                    p.Id,
                    p.Name,
                    p.Barcode,
                    p.Description ?? string.Empty,
                    p.Price,
                    p.Image,
                    p.Quantity,
                    DateTimeOffset.UtcNow,
                    ProductStatus.Enabled,
                    p.CategoryId,
                    "Category")).ToList());

        // Act
        var result = await _productService.GetAllProductsAsync(
            10,
            1,
            name: "Phone");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Phone", result.First().Name);
    }

    [Fact]
    public async Task GetAllProducts_Should_Filter_By_Price_Range()
    {
        var query = GetProductQuery();

        _unitOfWorkMock.Setup(x => x.Products.GetAll())
            .Returns(query);

        _unitOfWorkMock.Setup(x => x.Products.ToPagedList(
                It.IsAny<IQueryable<Product>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(query.ToList());

        var result = await _productService.GetAllProductsAsync(
            10, 1,
            priceFrom: 100,
            priceTo: 1000);

        Assert.All(result, x =>
            Assert.InRange(x.Price, 100, 1000));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetAllProducts_Should_Filter_By_Image(bool hasImage)
    {
        var query = GetProductQuery();

        _unitOfWorkMock.Setup(x => x.Products.GetAll())
            .Returns(query);

        _unitOfWorkMock.Setup(x => x.Products.ToPagedList(
                It.IsAny<IQueryable<Product>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(query.ToList());

        var result = await _productService.GetAllProductsAsync(
            10,
            1,
            hasImage: hasImage);

        if (hasImage)
        {
            Assert.All(result, x => Assert.False(string.IsNullOrWhiteSpace(x.Image)));
        }
        else
        {
            Assert.All(result, x => Assert.True(string.IsNullOrWhiteSpace(x.Image)));
        }
    }

    [Fact]
    public async Task GetProductByIdAsync_Should_Return_Product_When_Exists()
    {
        // Arrange
        const int productId = 1;

        var product = Product.Create(
            "Product 1",
            "123456789",
            "Description 1",
            10,
            "image 1",
            1,
            1);

        _unitOfWorkMock
            .Setup(x => x.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(x => x.Map<ProductDTO>(It.IsAny<Product>()))
            .Returns(new ProductDTO(
                productId,
                product.Name,
                product.Barcode,
                product.Description!,
                product.Price,
                product.Image,
                product.Quantity,
                DateTimeOffset.UtcNow,
                ProductStatus.Enabled,
                product.CategoryId,
                "Category 1"));

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);

        _unitOfWorkMock.Verify(
            x => x.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ProductDTO>(It.IsAny<Product>()),
            Times.Once);
    }

    [Fact]
    public async Task GetProductByIdAsync_Should_Throw_Exception()
    {
        // Arrange
        const int productId = 1;

        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        async Task Act() => await _productService.GetProductByIdAsync(productId);

        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(Act);

        _unitOfWorkMock.Verify(
            uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetProductsByIdsAsync_Should_Return_Products_When_All_Exist()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("Phone", "111", "Good phone", 500, "img", 10, 1),
            Product.Create("Laptop", "222", "Good laptop", 1500, "img", 5, 1)
        };

        // Set IDs via reflection
        typeof(Product).GetProperty("Id")!.SetValue(products[0], 1);
        typeof(Product).GetProperty("Id")!.SetValue(products[1], 2);

        var command = new GetProductsByIdsCommand { ProductIds = new List<int> { 1, 2 } };

        _unitOfWorkMock
            .Setup(x => x.Products.GetByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
            .Returns((IEnumerable<Product> p) => p.Select(product =>
                new ProductDTO(
                    product.Id,
                    product.Name,
                    product.Barcode,
                    product.Description ?? string.Empty,
                    product.Price,
                    product.Image,
                    product.Quantity,
                    DateTimeOffset.UtcNow,
                    ProductStatus.Enabled,
                    product.CategoryId,
                    "Category")).ToList());

        // Act
        var result = await _productService.GetProductsByIdsAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        _unitOfWorkMock.Verify(
            x => x.Products.GetByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetProductsByIdsAsync_Should_Throw_When_Any_Product_Missing()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("Phone", "111", "Good phone", 500, "img", 10, 1)
        };

        typeof(Product).GetProperty("Id")!.SetValue(products[0], 1);

        var command = new GetProductsByIdsCommand { ProductIds = new List<int> { 1, 2, 3 } };

        _unitOfWorkMock
            .Setup(x => x.Products.GetByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        async Task Act() => await _productService.GetProductsByIdsAsync(command);

        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(Act);

        _unitOfWorkMock.Verify(
            x => x.Products.GetByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetProductsByIdsAsync_Should_Throw_When_Command_Is_Null()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _productService.GetProductsByIdsAsync(null!));
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
        const int quantity = 1;
        const int categoryId = 1;

        var category = Category.Create("Category 1");

        _unitOfWorkMock
            .Setup(uow => uow.Products.IsUniqueAsync(barcode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _unitOfWorkMock
            .Setup(uow => uow.Products.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var createProductCommand = new CreateProductCommand
        {
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            Quantity = quantity,
            CategoryId = categoryId,
        };

        // Act
        await _productService.CreateProductAsync(createProductCommand);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Products.IsUniqueAsync(barcode, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(category), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Products.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
        const int quantity = 1;
        const int categoryId = 1;

        _unitOfWorkMock
            .Setup(uow => uow.Products.IsUniqueAsync(barcode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createProductCommand = new CreateProductCommand
        {
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            Quantity = quantity,
            CategoryId = categoryId,
        };

        // Act
        async Task Act() => await _productService.CreateProductAsync(createProductCommand);

        // Assert
        await Assert.ThrowsAsync<ProductBarcodeAlreadyExistsException>(Act);

        _unitOfWorkMock.Verify(
            uow => uow.Products.IsUniqueAsync(barcode, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Products.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
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
        const int quantity = 1;
        const int categoryId = 1;

        _unitOfWorkMock
            .Setup(uow => uow.Products.IsUniqueAsync(barcode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var createProductCommand = new CreateProductCommand
        {
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            Quantity = quantity,
            CategoryId = categoryId,
        };

        // Act
        async Task Act() => await _productService.CreateProductAsync(createProductCommand);

        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(Act);

        _unitOfWorkMock.Verify(
            uow => uow.Products.IsUniqueAsync(barcode, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Products.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateProductAsync_Should_Throw_When_Command_Is_Null()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _productService.CreateProductAsync(null!));
    }

    [Fact]
    public async Task GetAllProducts_Should_Filter_By_Barcode()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("Phone", "111", "desc", 10, "img", 1, 1),
            Product.Create("Laptop", "222", "desc", 10, "img", 1, 1),
            Product.Create("Mouse", "333", "desc", 10, "img", 1, 1),
        };

        _unitOfWorkMock.Setup(x => x.Products.GetAll())
            .Returns(products.AsQueryable());

        _unitOfWorkMock.Setup(x => x.Products.ToPagedList(
                It.IsAny<IQueryable<Product>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<Product> q, int pageSize, int pageNumber, CancellationToken ct)
                => q.ToList());

        _mapperMock.Setup(x => x.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
            .Returns((IEnumerable<Product> p) => p.Select(product =>
                new ProductDTO(
                    product.Id,
                    product.Name,
                    product.Barcode,
                    product.Description ?? string.Empty,
                    product.Price,
                    product.Image,
                    product.Quantity,
                    DateTimeOffset.UtcNow,
                    ProductStatus.Enabled,
                    product.CategoryId,
                    "Category")).ToList());

        // Act
        var result = await _productService.GetAllProductsAsync(10, 1, barcode: "222");

        // Assert
        Assert.Single(result);
        Assert.Equal("Laptop", result.First().Name);
    }

    [Fact]
    public async Task GetAllProducts_Should_Filter_By_Description()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("Phone", "111", "Good phone description", 10, "img", 1, 1),
            Product.Create("Laptop", "222", "Great laptop", 10, "img", 1, 1),
            Product.Create("Mouse", "333", null, 10, "img", 1, 1),
        };

        _unitOfWorkMock.Setup(x => x.Products.GetAll())
            .Returns(products.AsQueryable());

        _unitOfWorkMock.Setup(x => x.Products.ToPagedList(
                It.IsAny<IQueryable<Product>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<Product> q, int pageSize, int pageNumber, CancellationToken ct)
                => q.ToList());

        _mapperMock.Setup(x => x.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
            .Returns((IEnumerable<Product> p) => p.Select(product =>
                new ProductDTO(
                    product.Id,
                    product.Name,
                    product.Barcode,
                    product.Description ?? string.Empty,
                    product.Price,
                    product.Image,
                    product.Quantity,
                    DateTimeOffset.UtcNow,
                    ProductStatus.Enabled,
                    product.CategoryId,
                    "Category")).ToList());

        // Act
        var result = await _productService.GetAllProductsAsync(10, 1, description: "phone");

        // Assert
        Assert.Single(result);
        Assert.Equal("Phone", result.First().Name);
    }

    [Fact]
    public async Task GetAllProducts_Should_Filter_By_Multiple_Criteria()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("Phone", "111", "Good phone", 500, "img", 10, 1),
            Product.Create("Laptop", "222", "Good laptop", 1500, "img", 5, 1),
            Product.Create("Mouse", "333", "Small mouse", 20, null, 50, 1),
            Product.Create("Phone Plus", "444", "Better phone", 800, "img", 3, 1),
        };

        _unitOfWorkMock.Setup(x => x.Products.GetAll())
            .Returns(products.AsQueryable());

        _unitOfWorkMock.Setup(x => x.Products.ToPagedList(
                It.IsAny<IQueryable<Product>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<Product> q, int pageSize, int pageNumber, CancellationToken ct)
                => q.ToList());

        _mapperMock.Setup(x => x.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
            .Returns((IEnumerable<Product> p) => p.Select(product =>
                new ProductDTO(
                    product.Id,
                    product.Name,
                    product.Barcode,
                    product.Description ?? string.Empty,
                    product.Price,
                    product.Image,
                    product.Quantity,
                    DateTimeOffset.UtcNow,
                    ProductStatus.Enabled,
                    product.CategoryId,
                    "Category")).ToList());

        // Act - Filter by name "Phone" and price range 400-900
        var result = await _productService.GetAllProductsAsync(
            10, 1,
            name: "Phone",
            priceFrom: 400,
            priceTo: 900);

        // Assert - Should return only Phone (500) and Phone Plus (800), not Laptop (1500) or Mouse (20)
        Assert.Equal(2, result.Count());
        Assert.All(result, x => Assert.Contains("Phone", x.Name));
        Assert.All(result, x => Assert.InRange(x.Price, 400, 900));
    }

    [Fact]
    public async Task GetAllProducts_Should_Return_Empty_When_No_Match()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("Phone", "111", "desc", 10, "img", 1, 1),
        };

        _unitOfWorkMock.Setup(x => x.Products.GetAll())
            .Returns(products.AsQueryable());

        _unitOfWorkMock.Setup(x => x.Products.ToPagedList(
                It.IsAny<IQueryable<Product>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<Product> q, int pageSize, int pageNumber, CancellationToken ct)
                => q.ToList());

        _mapperMock.Setup(x => x.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
            .Returns(new List<ProductDTO>());

        // Act
        var result = await _productService.GetAllProductsAsync(10, 1, name: "NonExistent");

        // Assert
        Assert.Empty(result);
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
        const int quantity = 1;
        const int categoryId = 1;

        var product = Product.Create(name, barcode, description, price, image, quantity, categoryId);

        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
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
            Quantity = quantity,
            CategoryId = categoryId,
        };

        // Act
        await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Products.IsUniqueAsync(barcode, It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Products.Update(It.IsAny<Product>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_Throw_Exception_When_Product_Does_Not_Exist()
    {
        // Arrange
        const int productId = 1;

        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var updateProductCommand = new UpdateProductCommand
        {
            Id = productId,
            Name = "Product 1",
            Barcode = "123456789",
            Description = "Description 1",
            Price = 10,
            Image = "Image 1",
            Quantity = 1,
            CategoryId = 1,
        };

        // Act
        async Task Act() => await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(Act);

        _unitOfWorkMock.Verify(
            uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Products.Update(It.IsAny<Product>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
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
        const int quantity = 1;
        const int oldCategoryId = 1;
        var oldCategory = Category.Create("Category 1");
        const int newCategoryId = 2;
        var newCategory = Category.Create("Category 2");

        var product = Product.Create(name, barcode, description, price, image, quantity, oldCategoryId);

        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(oldCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldCategory);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(newCategoryId, It.IsAny<CancellationToken>()))
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
            Quantity = quantity,
            CategoryId = newCategoryId,
        };

        // Act
        await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(newCategoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(oldCategoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Exactly(2));
        _unitOfWorkMock.Verify(
            uow => uow.Products.Update(It.IsAny<Product>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(CancellationToken.None),
            Times.Once);
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
        const int quantity = 1;
        const int oldCategoryId = 1;
        const int newCategoryId = 2;

        var product = Product.Create(name, barcode, description, price, image, quantity, oldCategoryId);

        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(newCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var updateProductCommand = new UpdateProductCommand
        {
            Id = productId,
            Name = name,
            Barcode = barcode,
            Description = description,
            Price = price,
            Image = image,
            Quantity = quantity,
            CategoryId = newCategoryId,
        };

        // Act
        async Task Act() => await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(Act);

        _unitOfWorkMock.Verify(
            uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(newCategoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(oldCategoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Products.IsUniqueAsync(barcode, It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Products.Update(It.IsAny<Product>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_Handle_Null_Old_Category_When_Changing_Category()
    {
        // Arrange - Tests ProductService.cs lines 146-152
        // When changing category but old category doesn't exist (returns null)
        const int productId = 1;
        const string name = "Product 1";
        const string barcode = "123456789";
        const string description = "Description 1";
        const decimal price = 10;
        const string image = "Image 1";
        const int quantity = 1;
        const int oldCategoryId = 1;
        const int newCategoryId = 2;
        var newCategory = Category.Create("Category 2");

        var product = Product.Create(name, barcode, description, price, image, quantity, oldCategoryId);

        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        // Old category returns null - should be skipped (line 148 check)
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(oldCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);
        // New category exists
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(newCategoryId, It.IsAny<CancellationToken>()))
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
            Quantity = quantity,
            CategoryId = newCategoryId,
        };

        // Act
        await _productService.UpdateProductAsync(updateProductCommand);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
        // Old category should be fetched but since it's null, not updated
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(oldCategoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        // New category should be fetched and updated
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(newCategoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(newCategory),
            Times.Once); // Only new category updated
        _unitOfWorkMock.Verify(
            uow => uow.Products.Update(It.IsAny<Product>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_Should_Succeed()
    {
        // Arrange
        const int productId = 1;
        var product = Product.Create("Product 1", "123456789", "Description 1", 10, "Image 1", 1, 1);
        var category = Category.Create("Category 1");

        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(product.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _unitOfWorkMock.Setup(uow => uow.Products.DeleteAsync(productId, It.IsAny<CancellationToken>()));

        // Act
        await _productService.DeleteProductAsync(productId);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(product.CategoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Products.DeleteAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_Should_Throw_Exception_When_Product_Does_Not_Exist()
    {
        // Arrange
        const int productId = 1;

        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        async Task Act() => await _productService.DeleteProductAsync(productId);

        // Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(Act);

        _unitOfWorkMock.Verify(
            uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Products.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteProductAsync_Should_Throw_Exception_When_Category_Does_Not_Exist()
    {
        // Arrange
        const int productId = 1;

        var product = Product.Create("Product 1", "123456789", "Description 1", 10, "Image 1", 1, 1);

        _unitOfWorkMock
            .Setup(uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(product.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        async Task Act() => await _productService.DeleteProductAsync(productId);

        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(Act);

        _unitOfWorkMock.Verify(
            uow => uow.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(product.CategoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Products.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DecreaseProductQuantity_Should_Decrease_Quantity_And_Save()
    {
        // Arrange
        var product = Product.Create(
            "P1",
            "111",
            "desc",
            10,
            "img",
            10,
            1);

        // IMPORTANT: ensure Id matches command
        typeof(Product)
            .GetProperty("Id")!
            .SetValue(product, 1);

        _unitOfWorkMock
            .Setup(x => x.Products.GetByIdsAsync(
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { product });

        var command = new DecreaseProductsQuantityCommand
        {
            Items = new List<ProductQuantityDto>
            {
                new ProductQuantityDto
                {
                    ProductId = 1,
                    Quantity = 3
                }
            }
        };

        // Act
        await _productService.DecreaseProductQuantityAsync(command);

        // Assert
        Assert.Equal(7, product.Quantity);

        _unitOfWorkMock.Verify(
            x => x.Products.Update(product),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DecreaseProductQuantity_Should_Throw_When_Not_Enough_Quantity()
    {
        // Arrange
        var product = Product.Create(
            "P1",
            "111",
            "desc",
            2,
            "img",
            2,
            1);

        typeof(Product)
            .GetProperty("Id")!
            .SetValue(product, 1);

        _unitOfWorkMock
            .Setup(x => x.Products.GetByIdsAsync(
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([product]);

        var command = new DecreaseProductsQuantityCommand
        {
            Items = new List<ProductQuantityDto>
            {
                new ProductQuantityDto
                {
                    ProductId = 1,
                    Quantity = 5 // more than available (2)
                }
            }
        };

        // Act
        async Task Act()
            => await _productService.DecreaseProductQuantityAsync(command);

        // Assert
        await Assert.ThrowsAsync<InsufficientProductQuantityException>(Act);

        _unitOfWorkMock.Verify(
            x => x.Products.Update(It.IsAny<Product>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DecreaseProductQuantity_Should_Throw_When_Product_Not_Found()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Products.GetByIdsAsync(
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var command = new DecreaseProductsQuantityCommand
        {
            Items = new List<ProductQuantityDto>
            {
                new() { ProductId = 999, Quantity = 1 }
            }
        };

        // Act + Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _productService.DecreaseProductQuantityAsync(command));
    }

    [Fact]
    public async Task IncreaseProductQuantity_Should_Increase_Quantity_And_Save()
    {
        // Arrange
        var product = Product.Create("P1", "111", "desc", 10, "img", 5, 1);

        typeof(Product)
            .GetProperty("Id")!
            .SetValue(product, 1);

        _unitOfWorkMock.Setup(x => x.Products.GetByIdsAsync(
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([product]);

        var command = new IncreaseProductsQuantityCommand
        {
            Items = new List<ProductQuantityDto>
            {
                new() { ProductId = 1, Quantity = 4 }
            }
        };

        // Act
        await _productService.IncreaseProductsQuantityAsync(command);

        // Assert
        Assert.Equal(9, product.Quantity); // 5 + 4

        _unitOfWorkMock.Verify(x => x.Products.Update(product), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IncreaseProductQuantity_Should_Throw_When_Product_Not_Found()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Products.GetByIdsAsync(
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var command = new IncreaseProductsQuantityCommand
        {
            Items = new List<ProductQuantityDto>
            {
                new() { ProductId = 1, Quantity = 1 }
            }
        };

        // Act + Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _productService.IncreaseProductsQuantityAsync(command));
    }
}
