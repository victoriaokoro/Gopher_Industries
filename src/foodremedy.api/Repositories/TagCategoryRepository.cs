using foodremedy.database;
using foodremedy.database.Models;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.api.Repositories;

public interface ITagCategoryRepository
{
    Task<PaginatedResult<TagCategory>> GetAsync(int skip = 0, int take = 20);
    TagCategory Add(TagCategory tagCategory);
    Task<TagCategory?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
    Task<TagCategory?> GetByName(string name);
}

public class TagCategoryRepository : ITagCategoryRepository
{
    private readonly FoodRemedyDbContext _dbContext;

    public TagCategoryRepository(FoodRemedyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResult<TagCategory>> GetAsync(int skip = 0, int take = 20)
    {
        List<TagCategory> result = await _dbContext
            .TagCategory
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return new PaginatedResult<TagCategory>(result.Count, _dbContext.TagCategory.Count(), result);
    }

    public TagCategory Add(TagCategory tagCategory)
    {
        return _dbContext.TagCategory.Add(tagCategory).Entity;
    }

    public async Task<TagCategory?> GetByIdAsync(Guid id)
    {
        return await _dbContext.TagCategory.SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TagCategory?> GetByName(string name)
    {
        return await _dbContext
            .TagCategory
            .Include(p => p.Tags)
            .SingleOrDefaultAsync(p => p.Name.Equals(name));
    }
}
