using DigitalBank.API.DB;

namespace DigitalBank.API.Repositories;

public abstract class BaseRepository(DigitalBankDbContext dbContext)
{
    protected DigitalBankDbContext DbContext => dbContext;

    public async Task<T> FindAsync<T>(int id, CancellationToken cancellationToken)
        where T : class
    {
        return await DbContext.Set<T>().FindAsync([id], cancellationToken: cancellationToken);
    }

    protected async Task AddAsync<T>(T entity, CancellationToken cancellationToken)
        where T : class
    {
        await DbContext.Set<T>().AddAsync(entity, cancellationToken);
    }

    protected void Update<T>(T entity)
        where T : class
    {
        DbContext.Set<T>().Update(entity);
    }

    protected async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
