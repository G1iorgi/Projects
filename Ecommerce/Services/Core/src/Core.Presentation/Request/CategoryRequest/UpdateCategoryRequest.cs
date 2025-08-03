using Ardalis.GuardClauses;
using Core.Application.Aggregates.CategoryAggregate.Commands;

namespace Core.Presentation.Request.CategoryRequest;

public record UpdateCategoryRequest
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public static UpdateCategoryCommand ToCommand(UpdateCategoryRequest? request)
    {
        Guard.Against.Null(request);
        Guard.Against.NegativeOrZero(request.Id);
        Guard.Against.NullOrWhiteSpace(request.Name);

        return new UpdateCategoryCommand
        {
            Id = request.Id,
            Name = request.Name,
        };
    }
}
