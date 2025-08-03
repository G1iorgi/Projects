namespace Core.Domain;

public interface IGenericRepository<T>
    where T : class
{
    IQueryable<T> GetAll();

    Task<List<T>> GetAllAsync(
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken = default);

    Task<List<T>> ToPagedList(
        IQueryable<T> source,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
