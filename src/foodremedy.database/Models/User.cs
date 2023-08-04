namespace foodremedy.database.Models;

public class User
{
    public User(string email, string passwordHash, string passwordSalt)
    {
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public Guid Id { get; init; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public Guid? RefreshTokenId { get; set; }
}
