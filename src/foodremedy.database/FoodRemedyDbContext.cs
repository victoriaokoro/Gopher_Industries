using foodremedy.database.Extensions;
using foodremedy.database.Models;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.database;

public class FoodRemedyDbContext : DbContext
{
    public FoodRemedyDbContext()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Database.EnsureCreatedAsync().GetAwaiter().GetResult();
    }

    //TODO: Remove local db testing code
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseSqlite("Data Source=test.db");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ConfigureUsers();
        builder.ConfigureRefreshTokens();
        builder.ConfigureTags();
        builder.ConfigureIngredients();
    }
}
