using foodremedy.database.Models;

namespace foodremedy.api.Repositories;

public interface PaginatedRepository<TDbType> where TDbType : class
{
    Task<PaginatedResult<TDbType>> GetAsync(int skip = 0, int take = 20);
    TDbType Add(TDbType entry);
    Task SaveChangesAsync();
    Task<TDbType?> GetByIdAsync(Guid id);
}
