namespace foodremedy.database.Models;

public class RefreshToken
{
    public RefreshToken(string token)
    {
        Token = token;
    }

    public Guid Id { get; init; }
    public string Token { get; set; }
}
