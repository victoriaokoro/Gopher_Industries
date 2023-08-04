using foodremedy.database;
using foodremedy.database.Models;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.api.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    User AddUser(User user);
    Task SaveChangesAsync();
    Task<User?> GetUserByIdAsync(string userId);
}

public class UserRepository : IUserRepository
{
    private readonly FoodRemedyDbContext _dbContext;

    public UserRepository(FoodRemedyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(p => p.Email.Equals(email));
    }

    public User AddUser(User user)
    {
        return _dbContext.Users.Add(user).Entity;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        if (!Guid.TryParse(userId, out Guid id))
            return null;

        return await _dbContext.Users.SingleOrDefaultAsync(p => p.Id == id);
    }
}
