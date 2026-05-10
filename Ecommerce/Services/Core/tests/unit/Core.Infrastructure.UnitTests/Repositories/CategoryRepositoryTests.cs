using Core.Domain.Aggregates.CategoryAggregate;
using Core.Infrastructure.DbContexts;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.UnitTests.Repositories;

public class CategoryRepositoryTests : IDisposable
{
    private readonly CoreDbContextMaster _dbContext;
    private readonly CategoryRepository _repository;

    public CategoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CoreDbContextMaster>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CoreDbContextMaster(options);
        _repository = new CategoryRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task IsUniqueAsync_ShouldReturnTrue_WhenNameIsUnique()
    {
        // Arrange
        const string uniqueName = "UniqueCategory";

        // Act
        var result = await _repository.IsUniqueAsync(uniqueName, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsUniqueAsync_ShouldReturnFalse_WhenNameIsNotUnique()
    {
        // Arrange
        const string existingName = "ExistingCategory";
        var category = Category.Create(existingName);
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.IsUniqueAsync(existingName, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsUniqueAsync_ShouldReturnTrue_WhenNameIsUnique_AfterAddingDifferentName()
    {
        // Arrange
        const string existingName = "ExistingCategory";
        const string uniqueName = "UniqueCategory";
        var category = Category.Create(existingName);
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.IsUniqueAsync(uniqueName, CancellationToken.None);

        // Assert
        Assert.True(result);
    }
}
