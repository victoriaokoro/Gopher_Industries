using foodremedy.database;
using foodremedy.database.Models;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.api.Repositories;

public interface ITagRepository
{
    Task<List<Tag>> GetAllAsync();
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

    public async Task<List<Tag>> GetAllAsync()
    {
        return await _dbContext.Tags.ToListAsync();
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
