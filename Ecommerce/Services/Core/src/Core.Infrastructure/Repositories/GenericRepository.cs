using Core.Domain;
using Core.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repositories;

public class GenericRepository<T>(CoreDbContextMaster coreDbContext) : IGenericRepository<T>
    where T : class
{
    protected CoreDbContextMaster CoreDbContext => coreDbContext;

    public virtual IQueryable<T> GetAll() => CoreDbContext.Set<T>();

    public async Task<List<T>> GetAllAsync(
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken = default)
    {
        var query = CoreDbContext.Set<T>();
        return await ToPagedList(query, pageSize, pageNumber, cancellationToken);
    }

    public async Task<List<T>> ToPagedList(
        IQueryable<T> source,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken = default)
        => await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await CoreDbContext.Set<T>().FindAsync([id], cancellationToken);

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
        => await CoreDbContext.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) => CoreDbContext.Set<T>().Update(entity);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await CoreDbContext.Set<T>().FindAsync([id], cancellationToken);
        if (entity != null)
        {
            CoreDbContext.Set<T>().Remove(entity);
        }
    }
}
