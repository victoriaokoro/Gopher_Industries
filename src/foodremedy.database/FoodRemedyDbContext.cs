using foodremedy.database.Extensions;
using foodremedy.database.Models;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.database;

public class FoodRemedyDbContext : DbContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public FoodRemedyDbContext()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Database.EnsureCreatedAsync().GetAwaiter().GetResult();
    }

    //TODO: Remove local db testing code
    public DbSet<User> User { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }
    public DbSet<Tag> Tag { get; set; }
    public DbSet<Ingredient> Ingredient { get; set; }
    public DbSet<TagCategory> TagCategory { get; set; }

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
        builder.ConfigureTagCategories();
    }
}
