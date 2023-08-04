using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using foodremedy.api.Configuration;
using foodremedy.api.Repositories;
using foodremedy.api.Utils;
using foodremedy.database.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace foodremedy.api.Providers;

public interface IAuthenticationProvider
{
    bool UserCanLogin(User user, string password);
    JwtSecurityToken CreateAccessToken(User user);
    Task<RefreshToken> RefreshRefreshTokenAsync(User user);
    Task<bool> RefreshTokenIsValidAsync(User user, string refreshToken);
}

public class AuthenticationProvider : IAuthenticationProvider
{
    private readonly AuthenticationConfiguration _authenticationConfiguration;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthenticationProvider(IOptions<AuthenticationConfiguration> authenticationConfiguration,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _authenticationConfiguration = authenticationConfiguration.Value;
    }

    public async Task<RefreshToken> RefreshRefreshTokenAsync(User user)
    {
        string token = GenerateRefreshToken();

        if (user.RefreshTokenId == null)
        {
            var refreshToken = new RefreshToken(token);
            _refreshTokenRepository.Add(refreshToken);
            user.RefreshTokenId = refreshToken.Id;
            return refreshToken;
        }

        RefreshToken? existingToken = await _refreshTokenRepository.GetByUserAsync(user);

        ArgumentNullException.ThrowIfNull(existingToken);

        existingToken.Token = token;

        return existingToken;
    }

    public async Task<bool> RefreshTokenIsValidAsync(User user, string refreshToken)
    {
        RefreshToken? existingRefreshToken = await _refreshTokenRepository.GetByUserAsync(user);

        return existingRefreshToken != null && existingRefreshToken.Token.Equals(refreshToken);
    }

    public JwtSecurityToken CreateAccessToken(User user)
    {
        DateTime currentTime = DateTime.Now;

        var ttl = (double)_authenticationConfiguration.TokenTimeToLive;

        return new JwtSecurityToken(
            _authenticationConfiguration.Issuer,
            _authenticationConfiguration.Audience,
            GetClaims(user, currentTime),
            currentTime,
            currentTime.AddSeconds(ttl),
            GetSigningCredentials());
    }

    public bool UserCanLogin(User user, string password)
    {
        return user.PasswordHash.Equals(HashString(password, user.PasswordSalt));
    }

    private SigningCredentials GetSigningCredentials()
    {
        ArgumentException.ThrowIfNullOrEmpty(_authenticationConfiguration.SigningKey);
        return new SigningCredentials(SigningKeyFactory.Get(_authenticationConfiguration.SigningKey),
            SecurityAlgorithms.HmacSha256);
    }

    private static IEnumerable<Claim> GetClaims(User user, DateTimeOffset issuedAt)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, issuedAt.ToUnixTimeSeconds().ToString())
        };

        return claims;
    }

    private static string HashString(string accessToken, string salt)
    {
        return StringHasher.Hash(accessToken, salt);
    }

    private static string GenerateRefreshToken()
    {
        byte[] tknBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(tknBytes)
            .Replace("+", "-")
            .Replace("/", "_");
    }
}
