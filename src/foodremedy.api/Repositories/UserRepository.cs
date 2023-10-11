using foodremedy.database;
using foodremedy.database.Models;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    User Add(User user);
    Task SaveChangesAsync();
    Task<User?> GetByIdAsync(string userId);
}

public class UserRepository : IUserRepository
{
    private readonly FoodRemedyDbContext _dbContext;

    public UserRepository(FoodRemedyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.User.SingleOrDefaultAsync(p => p.Email.Equals(email));
    }

    public User Add(User user)
    {
        return _dbContext.User.Add(user).Entity;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(string userId)
    {
        if (!Guid.TryParse(userId, out Guid id))
            return null;

        return await _dbContext.User.SingleOrDefaultAsync(p => p.Id == id);
    }
}
