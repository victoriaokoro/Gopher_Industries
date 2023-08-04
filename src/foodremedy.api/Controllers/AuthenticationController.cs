using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using foodremedy.api.Providers;
using foodremedy.api.Repositories;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodremedy.api.Controllers;

[ApiController]
[Route("auth")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IUserRepository _userRepository;

    public AuthenticationController(IUserRepository userRepository, IAuthenticationProvider authenticationProvider)
    {
        _userRepository = userRepository;
        _authenticationProvider = authenticationProvider;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenCreated>> AttemptLogin([FromBody] AttemptLogin attemptLogin)
    {
        User? user = await _userRepository.GetUserByEmailAsync(attemptLogin.Email);

        if (user == null || !_authenticationProvider.UserCanLogin(user, attemptLogin.Password))
            return Unauthorized();

        RefreshToken refreshToken = await _authenticationProvider.RefreshRefreshTokenAsync(user);
        ArgumentNullException.ThrowIfNull(user.RefreshTokenId);
        await _userRepository.SaveChangesAsync();

        JwtSecurityToken accessToken = _authenticationProvider.CreateAccessToken(user);

        return Ok(BuildAccessTokenCreatedResponse(accessToken, refreshToken.Token));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AccessTokenCreated>> RefreshAccessToken(
        [FromBody] RefreshAccessToken refreshAccessToken)
    {
        Claim? userId = User.Claims.SingleOrDefault(p => p.Type.Equals(JwtRegisteredClaimNames.Sub));

        if (userId == null)
            return Unauthorized();

        User? user = await _userRepository.GetUserByIdAsync(userId.Value);

        if (user == null || !await _authenticationProvider.RefreshTokenIsValidAsync(user, refreshAccessToken.RefreshToken))
            return Unauthorized();

        RefreshToken refreshToken = await _authenticationProvider.RefreshRefreshTokenAsync(user);
        ArgumentNullException.ThrowIfNull(user.RefreshTokenId);
        await _userRepository.SaveChangesAsync();

        JwtSecurityToken accessToken = _authenticationProvider.CreateAccessToken(user);

        return Ok(BuildAccessTokenCreatedResponse(accessToken, refreshToken.Token));
    }
    
    private static AccessTokenCreated BuildAccessTokenCreatedResponse(JwtSecurityToken accessToken, string refreshToken)
    {
        return new AccessTokenCreated(
            "Bearer",
            new JwtSecurityTokenHandler().WriteToken(accessToken),
            (int)(accessToken.ValidTo - DateTime.UtcNow).TotalSeconds,
            refreshToken
        );
    }
}
