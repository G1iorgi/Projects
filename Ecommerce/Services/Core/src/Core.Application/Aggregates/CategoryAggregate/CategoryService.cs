using Ardalis.GuardClauses;
using Core.Application.Aggregates.CategoryAggregate.Commands;
using Core.Domain;
using Core.Domain.Aggregates.CategoryAggregate;

namespace Core.Application.Aggregates.CategoryAggregate;

public class CategoryService(IUnitOfWork unitOfWork)
{
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(
        int pageSize,
        int pageNumber,
        string? name = null,
        int? productQuantityFrom = null,
        int? productQuantityTo = null,
        CancellationToken cancellationToken = default)
    {
        var categories = unitOfWork.Categories.GetAll();

        if (!string.IsNullOrWhiteSpace(name))
        {
            categories = categories.Where(c => c.Name.Contains(name));
        }

        if (productQuantityFrom is > 0)
        {
            categories = categories.Where(c => c.Products.Count >= productQuantityFrom);
        }

        if (productQuantityTo is > 0)
        {
            categories = categories.Where(c => c.Products.Count <= productQuantityTo);
        }

        return await unitOfWork.Categories.ToPagedList(
            categories,
            pageSize,
            pageNumber,
            cancellationToken);
    }

    public async Task CreateCategoryAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        var isUnique = await unitOfWork.Categories.IsUniqueAsync(command.Name, cancellationToken);
        if (!isUnique)
        {
            throw new ArgumentException($"A category with the name '{command.Name}' already exists.");
        }

        var category = Category.Create(command.Name);

        await unitOfWork.Categories.CreateAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Category> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);

        if (category == null)
        {
            throw new ArgumentException($"Category with ID {categoryId} does not exist.");
        }

        return category;
    }

    public async Task UpdateCategoryAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        var category = await unitOfWork.Categories.GetByIdAsync(command.Id, cancellationToken);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {command.Id} does not exist.");
        }

        if (!category.NameHasChanged(command.Name))
        {
            return;
        }

        var isUnique = await unitOfWork.Categories.IsUniqueAsync(command.Name, cancellationToken);
        if (!isUnique)
        {
            throw new ArgumentException($"A category with the name '{command.Name}' already exists.");
        }

        category.UpdateMetadata(command.Name);
        unitOfWork.Categories.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.Categories.DeleteAsync(categoryId, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
