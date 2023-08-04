using foodremedy.database.Models;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.database.Extensions;

public static class ModelBuilderExtensions
{
    public static void ConfigureUser(this ModelBuilder builder)
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

    public static void ConfigureRefreshToken(this ModelBuilder builder)
    {
        builder.Entity<RefreshToken>(model =>
        {
            model.HasKey(p => p.Id);
            model.Property(p => p.Token).IsRequired();
            model.HasIndex(p => p.Token).IsUnique();
        });
    }
}
