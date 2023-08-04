namespace foodremedy.api.Models.Responses;

public record AccessTokenCreated(string TokenType, string AccessToken, int ExpiresIn, string RefreshToken);
