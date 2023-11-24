using foodremedy.database;
using foodremedy.database.Models;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.api.Repositories;

public interface IIngredientRepository
{
    Task<PaginatedResult<Ingredient>> GetAsync(int skip = 0, int take = 20);
    Ingredient Add(Ingredient ingredient);
    Task SaveChangesAsync();
    Task<Ingredient?> GetByIdAsync(Guid id);
}

public class IngredientRepository : IIngredientRepository
{
    private readonly FoodRemedyDbContext _dbContext;

    public IngredientRepository(FoodRemedyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResult<Ingredient>> GetAsync(int skip = 0, int take = 20)
    {
        List<Ingredient> result = await _dbContext
            .Ingredient
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return new PaginatedResult<Ingredient>(result.Count, _dbContext.Ingredient.Count(), result);
    }

    public Ingredient Add(Ingredient ingredient)
    {
        return _dbContext.Ingredient.Add(ingredient).Entity;
    }

    public async Task<Ingredient?> GetByIdAsync(Guid id)
    {
        return await _dbContext
            .Ingredient
            .Include(p => p.Tags)
            .ThenInclude(p => p.TagCategory)
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
