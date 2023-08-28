namespace foodremedy.api.Repositories;

public interface IEntityRepository<TDbType> where TDbType : class
{
    Task<TDbType> GetByIdAsync(Guid id);
    TDbType Add(TDbType entry);
    Task SaveChangesAsync();
}
