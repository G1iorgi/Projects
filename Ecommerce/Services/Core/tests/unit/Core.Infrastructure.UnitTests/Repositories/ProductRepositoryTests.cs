using Core.Domain.Aggregates.ProductAggregate;
using Core.Infrastructure.DbContexts;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.UnitTests.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly CoreDbContextMaster _dbContext;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CoreDbContextMaster>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CoreDbContextMaster(options);
        _repository = new ProductRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task IsUniqueAsync_ShouldReturnTrue_WhenBarcodeIsUnique()
    {
        // Arrange
        const string uniqueBarcode = "UNIQUE_BARCODE";

        // Act
        var result = await _repository.IsUniqueAsync(uniqueBarcode, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsUniqueAsync_ShouldReturnFalse_WhenBarcodeIsNotUnique()
    {
        // Arrange
        const string existingBarcode = "EXISTING_BARCODE";
        var product = Product.Create("Product Name",
            existingBarcode,
            "Product Description",
            10.0m,
            "image.jpg",
            100,
            1);
        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.IsUniqueAsync(existingBarcode, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetByIdsAsync_ShouldReturnProducts_WhenIdsExist()
    {
        // Arrange
        var product1 = Product.Create("Product 1", "BARCODE1", "Description 1", 10.0m, "image1.jpg", 100, 1);
        var product2 = Product.Create("Product 2", "BARCODE2", "Description 2", 20.0m, "image2.jpg", 200, 1);
        await _dbContext.Products.AddRangeAsync(product1, product2);
        await _dbContext.SaveChangesAsync();

        var ids = new List<int> { product1.Id, product2.Id };

        // Act
        var result = await _repository.GetByIdsAsync(ids, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Id == product1.Id);
        Assert.Contains(result, p => p.Id == product2.Id);
    }

    [Fact]
    public async Task GetByIdsAsync_ShouldReturnEmptyList_WhenIdsDoNotExist()
    {
        // Arrange
        var ids = new List<int> { 999, 1000 };

        // Act
        var result = await _repository.GetByIdsAsync(ids, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdsAsync_ShouldReturnEmptyList_WhenIdsAreEmpty()
    {
        // Arrange
        var ids = new List<int>();

        // Act
        var result = await _repository.GetByIdsAsync(ids, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
