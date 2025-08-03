namespace Core.Application.Aggregates.CategoryAggregate.Commands;

public record CreateCategoryCommand
{
    public required string Name { get; init; }
}
