using foodremedy.database;
using foodremedy.database.Models;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.api.Repositories;

public interface ITagRepository
{
    Task<PaginatedResult<Tag>> GetAsync(int skip = 0, int take = 20);
    Tag Add(Tag tag);
    Task SaveChangesAsync();
}

public class TagRepository : ITagRepository
{
    private readonly FoodRemedyDbContext _dbContext;

    public TagRepository(FoodRemedyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResult<Tag>> GetAsync(int skip = 0, int take = 20)
    {
        var results = await _dbContext
            .Tags
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return new PaginatedResult<Tag>(results.Count, _dbContext.Tags.Count(), results);
    }

    public Tag Add(Tag tag)
    {
        return _dbContext.Add(tag).Entity;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
