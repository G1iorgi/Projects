namespace Core.Domain.Aggregates.CategoryAggregate;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<bool> IsUniqueAsync(
        string name,
        CancellationToken cancellationToken = default);
}
