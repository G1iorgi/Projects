using Core.Application.Aggregates.CategoryAggregate;
using Core.Application.Aggregates.CategoryAggregate.Commands;
using Core.Domain;
using Core.Domain.Aggregates.CategoryAggregate;
using Moq;

namespace Core.Application.UnitTests;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryService = new CategoryService(_unitOfWorkMock.Object);
    }

    // [Fact]
    // public async Task GetAllCategoriesAsync_Should_Succeed()
    // {
    //     // Arrange
    //     var categories = new List<Category>
    //     {
    //         Category.Create("Category 1"),
    //         Category.Create("Category 2"),
    //         Category.Create("Category 3"),
    //     };
    //     unitOfWorkMock
    //         .Setup(uow => uow.Categories.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), null, default))
    //         .ReturnsAsync(categories);
    //
    //     // Act
    //     var result = await categoryService.GetAllCategoriesAsync(1, categories.Count);
    //
    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal(categories.Count, result.Count());
    //     unitOfWorkMock.Verify(
    //         uow => uow.Categories.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<CancellationToken>()),
    //         Times.Once);
    // }
    [Fact]
    public async Task GetCategoryByIdAsync_Should_Succeed()
    {
        // Arrange
        const int categoryId = 1;
        var category = Category.Create("Category 1");
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, CancellationToken.None))
            .ReturnsAsync(category);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Name, result.Name);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_Should_Throw_Exception()
    {
        // Arrange
        const int categoryId = 1;
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, CancellationToken.None))
            .ReturnsAsync((Category)null!);

        // Act
        async Task Act() => await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_Should_Succeed()
    {
        // Arrange
        var command = new CreateCategoryCommand { Name = "Category 1" };
        _unitOfWorkMock
            .Setup(uow => uow.Categories.IsUniqueAsync(command.Name, CancellationToken.None))
            .ReturnsAsync(true);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.CreateAsync(It.IsAny<Category>(), CancellationToken.None));

        // Act
        await _categoryService.CreateCategoryAsync(command);

        // Assert
        _unitOfWorkMock.Verify(
            uow =>
            uow.Categories.IsUniqueAsync(command.Name, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_Should_Throw_ArgumentException()
    {
        // Arrange
        var command = new CreateCategoryCommand { Name = "Category 1" };
        _unitOfWorkMock
            .Setup(uow => uow.Categories.IsUniqueAsync(command.Name, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        async Task Act() => await _categoryService.CreateCategoryAsync(command);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.IsUniqueAsync(command.Name, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_Throw_NullReferenceException()
    {
        // Arrange
        var command = new UpdateCategoryCommand { Id = 1, Name = "Category 1" };
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(command.Id, CancellationToken.None))
            .ReturnsAsync((Category)null!);

        // Act
        async Task Act() => await _categoryService.UpdateCategoryAsync(command);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(
            uow =>
            uow.Categories.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow =>
            uow.Categories.IsUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(x => x.Categories.Update(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_Succeed_Without_Change()
    {
        // Arrange
        const int categoryId = 1;
        const string name = "Category 1";
        var category = Category.Create(name);
        var command = new UpdateCategoryCommand { Id = categoryId, Name = name };
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, CancellationToken.None))
            .ReturnsAsync(category);

        // Act
        await _categoryService.UpdateCategoryAsync(command);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.IsUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_Succeed()
    {
        // Arrange
        const int categoryId = 1;
        const string oldName = "Category 1";
        const string newName = "Category 2";
        var oldCategory = Category.Create(oldName);
        var command = new UpdateCategoryCommand { Id = categoryId, Name = newName };
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, CancellationToken.None))
            .ReturnsAsync(oldCategory);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.IsUniqueAsync(command.Name, CancellationToken.None))
            .ReturnsAsync(true);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.Update(It.IsAny<Category>()));

        // Act
        await _categoryService.UpdateCategoryAsync(command);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.IsUniqueAsync(command.Name, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_Throw_ArgumentException()
    {
        // Arrange
        const int categoryId = 1;
        const string oldName = "Category 1";
        const string newName = "Category 2";
        var oldCategory = Category.Create(oldName);
        var command = new UpdateCategoryCommand { Id = categoryId, Name = newName };
        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(command.Id, CancellationToken.None))
            .ReturnsAsync(oldCategory);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.IsUniqueAsync(command.Name, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        async Task Act() => await _categoryService.UpdateCategoryAsync(command);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.IsUniqueAsync(command.Name, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteCategoryAsync_Should_Succeed()
    {
        // Arrange
        const int categoryId = 1;
        _unitOfWorkMock
            .Setup(uow => uow.Categories.DeleteAsync(categoryId, CancellationToken.None));

        // Act
        await _categoryService.DeleteCategoryAsync(categoryId);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.Categories.DeleteAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
