namespace Core.Application.UnitTests;

using Core.Application.Aggregates.CategoryAggregate;
using Core.Application.Aggregates.CategoryAggregate.Commands;
using Core.Domain;
using Core.Domain.Aggregates.CategoryAggregate;
using Moq;
using SharedKernel.Exceptions.Category;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryService = new CategoryService(_unitOfWorkMock.Object);
    }

    private static IQueryable<Category> GetCategoriesQuery()
    {
        return new List<Category>
        {
            Category.Create("Electronics"),
            Category.Create("Books"),
            Category.Create("Clothing"),
            Category.Create("Home & Garden"),
            Category.Create("Sports")
        }.AsQueryable();
    }

    [Fact]
    public async Task GetAllCategoriesAsync_Should_Return_All_Data()
    {
        var query = GetCategoriesQuery();

        _unitOfWorkMock.Setup(uow => uow.Categories.GetAll())
            .Returns(query);

        _unitOfWorkMock.Setup(uow => uow.Categories.ToPagedList(
                It.IsAny<IQueryable<Category>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(query.ToList());

        var result = await _categoryService.GetAllCategoriesAsync(10, 1);

        Assert.Equal(5, result.Count());

        _unitOfWorkMock.Verify(uow => uow.Categories.GetAll(), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.ToPagedList(
            It.IsAny<IQueryable<Category>>(), 10, 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_Should_Filter_By_Name()
    {
        // Arrange
        var categories = new List<Category>
        {
            Category.Create("Electronics"),
            Category.Create("Books"),
            Category.Create("Electronics Store"),
            Category.Create("Clothing")
        };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetAll())
            .Returns(categories.AsQueryable());

        // IMPORTANT: DO NOT simulate paging logic
        _unitOfWorkMock.Setup(uow => uow.Categories.ToPagedList(
                It.IsAny<IQueryable<Category>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<Category> q, int pageSize, int pageNumber, CancellationToken ct)
                => q.ToList());

        // Act
        var result = await _categoryService.GetAllCategoriesAsync(
            10,
            1,
            name: "Electronics");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.Contains("Electronics", c.Name));
    }

    [Fact]
    public async Task GetAllCategoriesAsync_Should_Filter_By_ProductQuantityFrom()
    {
        // Arrange
        var category1 = Category.Create("Category 1");
        var category2 = Category.Create("Category 2");
        var category3 = Category.Create("Category 3");

        // Add products to categories
        for (int i = 0; i < 5; i++) category1.AddProduct(new Core.Domain.Aggregates.ProductAggregate.Product());
        for (int i = 0; i < 3; i++) category2.AddProduct(new Core.Domain.Aggregates.ProductAggregate.Product());

        var categories = new List<Category> { category1, category2, category3 }.AsQueryable();

        _unitOfWorkMock.Setup(uow => uow.Categories.GetAll())
            .Returns(categories);

        // IMPORTANT: DO NOT simulate paging logic
        _unitOfWorkMock.Setup(uow => uow.Categories.ToPagedList(
                It.IsAny<IQueryable<Category>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<Category> q, int pageSize, int pageNumber, CancellationToken ct)
                => q.ToList());

        // Act
        var result = await _categoryService.GetAllCategoriesAsync(
            10, 1,
            productQuantityFrom: 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, x =>
            Assert.True(x.ProductQuantity >= 2));
    }

    [Fact]
    public async Task GetAllCategoriesAsync_Should_Filter_By_ProductQuantityTo()
    {
        // Arrange
        var category1 = Category.Create("Category 1");
        var category2 = Category.Create("Category 2");
        var category3 = Category.Create("Category 3");

        // Add products to categories
        for (int i = 0; i < 5; i++) category1.AddProduct(new Core.Domain.Aggregates.ProductAggregate.Product());
        for (int i = 0; i < 2; i++) category2.AddProduct(new Core.Domain.Aggregates.ProductAggregate.Product());

        var categories = new List<Category> { category1, category2, category3 }.AsQueryable();

        _unitOfWorkMock.Setup(uow => uow.Categories.GetAll())
            .Returns(categories);

        // IMPORTANT: DO NOT simulate paging logic
        _unitOfWorkMock.Setup(uow => uow.Categories.ToPagedList(
                It.IsAny<IQueryable<Category>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<Category> q, int pageSize, int pageNumber, CancellationToken ct)
                => q.ToList());

        // Act
        var result = await _categoryService.GetAllCategoriesAsync(
            10,
            1,
            productQuantityTo: 3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, x =>
            Assert.True(x.ProductQuantity <= 3));
    }

    [Fact]
    public async Task GetAllCategoriesAsync_Should_Filter_By_Name_And_ProductQuantityFrom()
    {
        // Arrange
        var category1 = Category.Create("Electronics");
        var category2 = Category.Create("Books");
        var category3 = Category.Create("Electronics Store");

        // Add products
        for (int i = 0; i < 5; i++) category1.AddProduct(new Core.Domain.Aggregates.ProductAggregate.Product());
        for (int i = 0; i < 3; i++) category2.AddProduct(new Core.Domain.Aggregates.ProductAggregate.Product());
        for (int i = 0; i < 2; i++) category3.AddProduct(new Core.Domain.Aggregates.ProductAggregate.Product());

        var categories = new List<Category> { category1, category2, category3 }.AsQueryable();

        _unitOfWorkMock.Setup(uow => uow.Categories.GetAll())
            .Returns(categories);

        // IMPORTANT: DO NOT simulate paging logic
        _unitOfWorkMock.Setup(uow => uow.Categories.ToPagedList(
                It.IsAny<IQueryable<Category>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<Category> q, int pageSize, int pageNumber, CancellationToken ct)
                => q.ToList());

        // Act
        var result = await _categoryService.GetAllCategoriesAsync(
            10,
            1,
            name: "Electronics",
            productQuantityFrom: 3);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result); // Only category1 matches both
        Assert.Equal("Electronics", result.First().Name);
        Assert.True(result.First().ProductQuantity >= 3);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_Should_Return_Category_When_Exists()
    {
        // Arrange
        const int categoryId = 1;

        var category = Category.Create(
            "Electronics");

        _unitOfWorkMock
            .Setup(x => x.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);

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
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        async Task Act() => await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(Act);

        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_Should_Succeed()
    {
        // Arrange
        const string name = "New Category";

        _unitOfWorkMock
            .Setup(uow => uow.Categories.IsUniqueAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var createCategoryCommand = new CreateCategoryCommand
        {
            Name = name,
        };

        // Act
        await _categoryService.CreateCategoryAsync(createCategoryCommand);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Categories.IsUniqueAsync(name, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_Should_Throw_Exception_When_Name_Is_Not_Unique()
    {
        // Arrange
        const string name = "Electronics";

        _unitOfWorkMock
            .Setup(uow => uow.Categories.IsUniqueAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createCategoryCommand = new CreateCategoryCommand
        {
            Name = name,
        };

        // Act
        async Task Act() => await _categoryService.CreateCategoryAsync(createCategoryCommand);

        // Assert
        await Assert.ThrowsAsync<CategoryAlreadyExistsException>(Act);

        _unitOfWorkMock.Verify(
            uow => uow.Categories.IsUniqueAsync(name, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateCategoryAsync_Should_Throw_When_Command_Is_Null()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _categoryService.CreateCategoryAsync(null!));
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_Succeed()
    {
        // Arrange
        const int categoryId = 1;
        const string oldName = "Electronics";
        const string newName = "Digital Devices";

        var category = Category.Create(oldName);

        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.IsUniqueAsync(newName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.Update(It.IsAny<Category>()));

        var updateCategoryCommand = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = newName,
        };

        // Act
        await _categoryService.UpdateCategoryAsync(updateCategoryCommand);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.IsUniqueAsync(newName, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_Throw_Exception_When_Category_Does_Not_Exist()
    {
        // Arrange
        const int categoryId = 999;
        const string newName = "New Name";

        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var updateCategoryCommand = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = newName,
        };

        // Act
        async Task Act() => await _categoryService.UpdateCategoryAsync(updateCategoryCommand);

        // Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(Act);

        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.IsUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_Throw_Exception_When_Name_Is_Not_Unique()
    {
        // Arrange
        const int categoryId = 1;
        const string oldName = "Electronics";
        const string newName = "Books";

        var category = Category.Create(oldName);

        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _unitOfWorkMock
            .Setup(uow => uow.Categories.IsUniqueAsync(newName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var updateCategoryCommand = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = newName,
        };

        // Act
        async Task Act() => await _categoryService.UpdateCategoryAsync(updateCategoryCommand);

        // Assert
        await Assert.ThrowsAsync<CategoryAlreadyExistsException>(Act);

        _unitOfWorkMock.Verify(
            uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.IsUniqueAsync(newName, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            uow => uow.Categories.Update(It.IsAny<Category>()),
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_Do_Nothing_When_Name_Is_Same()
    {
        // Arrange
        const int categoryId = 1;
        const string name = "Electronics";

        var category = Category.Create(name);

        _unitOfWorkMock
            .Setup(uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var updateCategoryCommand = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = name,
        };

        // Act
        await _categoryService.UpdateCategoryAsync(updateCategoryCommand);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Categories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.IsUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_Throw_When_Command_Is_Null()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _categoryService.UpdateCategoryAsync(null!));
    }

    [Fact]
    public async Task DeleteCategoryAsync_Should_Succeed()
    {
        // Arrange
        const int categoryId = 1;

        _unitOfWorkMock
            .Setup(uow => uow.Categories.DeleteAsync(categoryId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _categoryService.DeleteCategoryAsync(categoryId);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Categories.DeleteAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
