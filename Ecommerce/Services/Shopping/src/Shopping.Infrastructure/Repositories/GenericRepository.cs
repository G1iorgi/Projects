using Microsoft.EntityFrameworkCore;
using Shopping.Domain;
using Shopping.Infrastructure.DbContexts;

namespace Shopping.Infrastructure.Repositories;

public class GenericRepository<T>(ShoppingDbContextMaster shoppingDbContext) : IGenericRepository<T>
    where T : class
{
    protected ShoppingDbContextMaster ShoppingDbContext => shoppingDbContext;

    public virtual IQueryable<T> GetAll() => ShoppingDbContext.Set<T>();

    public async Task<List<T>> GetAllAsync(int pageSize,
        int pageNumber,
        CancellationToken cancellationToken = default)
    {
        var query = ShoppingDbContext.Set<T>();
        return await ToPagedList(query, pageSize, pageNumber, cancellationToken);
    }

    public async Task<List<T>> ToPagedList(IQueryable<T> source,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken = default)
        => await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await ShoppingDbContext.Set<T>().FindAsync([id], cancellationToken);

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
        => await ShoppingDbContext.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) => ShoppingDbContext.Set<T>().Update(entity);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await ShoppingDbContext.Set<T>().FindAsync([id], cancellationToken);
        if (entity != null)
            ShoppingDbContext.Set<T>().Remove(entity);
    }
}
