using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Category;

public sealed class CategoryAlreadyExistsException(string name) : ConflictException(
    $"A category with the name '{name}' already exists.",
    "CATEGORY_ALREADY_EXISTS");
