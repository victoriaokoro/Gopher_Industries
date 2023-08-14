using foodremedy.database.Models;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.database.Extensions;

public static class ModelBuilderExtensions
{
    public static void ConfigureUsers(this ModelBuilder builder)
    {
        builder.Entity<User>(model =>
        {
            model.HasKey(p => p.Id);
            model.Property(p => p.Email).IsRequired();
            model.Property(p => p.PasswordHash).IsRequired();
            model.HasIndex(p => p.Email).IsUnique();

            model.HasOne<RefreshToken>().WithOne();
        });
    }

    public static void ConfigureRefreshTokens(this ModelBuilder builder)
    {
        builder.Entity<RefreshToken>(model =>
        {
            model.HasKey(p => p.Id);
            model.Property(p => p.Token).IsRequired();
            model.HasIndex(p => p.Token).IsUnique();
        });
    }

    public static void ConfigureTags(this ModelBuilder builder)
    {
        builder.Entity<Tag>(model =>
        {
            model.HasKey(p => p.Id);
            model.Property(p => p.TagType).IsRequired();
            model.Property(p => p.Description).IsRequired();
        });
    }

    public static void ConfigureIngredients(this ModelBuilder builder)
    {
        builder.Entity<Ingredient>(model =>
        {
            model.HasKey(p => p.Id);
            model.Property(p => p.Description).IsRequired();
            model.HasMany<Tag>(p => p.SeasonTags).WithMany();
            model.HasMany<Tag>(p => p.ServingSizeTags).WithMany();
        });
    }
}
