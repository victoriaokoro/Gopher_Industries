using foodremedy.database;
using foodremedy.database.Models;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.api.Repositories;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);
    Task<RefreshToken?> GetByUserAsync(User user);
}

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly FoodRemedyDbContext _dbContext;

    public RefreshTokenRepository(FoodRemedyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(RefreshToken refreshToken)
    {
        _dbContext.RefreshToken.Add(refreshToken);
    }

    public async Task<RefreshToken?> GetByUserAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user.RefreshTokenId);
        return await _dbContext.RefreshToken.SingleOrDefaultAsync(p => p.Id == user.RefreshTokenId);
    }
}
