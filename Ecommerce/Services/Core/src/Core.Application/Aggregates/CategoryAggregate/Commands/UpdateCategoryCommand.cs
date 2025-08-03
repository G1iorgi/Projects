namespace Core.Application.Aggregates.CategoryAggregate.Commands;

public record UpdateCategoryCommand
{
    public required int Id { get; init; }

    public required string Name { get; init; }
}
