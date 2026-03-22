using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Category;

public sealed class CategoryNotFoundException(int categoryId) : NotFoundException(
    $"Category with ID {categoryId} was not found.",
    "CATEGORY_NOT_FOUND");
