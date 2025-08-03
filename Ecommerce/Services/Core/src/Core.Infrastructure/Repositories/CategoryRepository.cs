using Core.Domain.Aggregates.CategoryAggregate;
using Core.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repositories;

public class CategoryRepository(CoreDbContextMaster dbContext)
    : GenericRepository<Category>(dbContext), ICategoryRepository
{
    public async Task<bool> IsUniqueAsync(
        string name,
        CancellationToken cancellationToken = default)
        => !await CoreDbContext.Categories.AnyAsync(c => c.Name == name, cancellationToken);
}
