using Ardalis.GuardClauses;
using Core.Application.Aggregates.CategoryAggregate.Commands;

namespace Core.Presentation.Request.CategoryRequest;

public record CreateCategoryRequest
{
    public required string Name { get; init; }

    public static CreateCategoryCommand ToCommand(CreateCategoryRequest? request)
    {
        Guard.Against.Null(request);
        Guard.Against.NullOrWhiteSpace(request.Name);

        return new CreateCategoryCommand
        {
            Name = request.Name
        };
    }
}
