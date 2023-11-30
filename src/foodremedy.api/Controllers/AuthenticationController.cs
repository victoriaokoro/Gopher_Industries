﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using foodremedy.api.Extensions;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using foodremedy.api.Providers;
using foodremedy.api.Repositories;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using User = foodremedy.database.Models.User;

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
        User? user = await _userRepository.GetByEmailAsync(attemptLogin.Email);

        if (user == null || !_authenticationProvider.UserCanLogin(user, attemptLogin.Password))
            return Unauthorized();

        RefreshToken refreshToken = await _authenticationProvider.RefreshRefreshTokenAsync(user);
        ArgumentNullException.ThrowIfNull(user.RefreshTokenId);
        await _userRepository.SaveChangesAsync();

        JwtSecurityToken accessToken = _authenticationProvider.CreateAccessToken(user);

        return Ok(accessToken.ToResponseModel(refreshToken));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AccessTokenCreated>> RefreshAccessToken(
        [FromBody] RefreshAccessToken refreshAccessToken)
    {
        Claim? userId = User.Claims.SingleOrDefault(p => p.Type.Equals("subject"));

        if (userId == null)
            return Unauthorized();

        User? user = await _userRepository.GetByIdAsync(userId.Value);

        if (user == null ||
            !await _authenticationProvider.RefreshTokenIsValidAsync(user, refreshAccessToken.RefreshToken))
            return Unauthorized();

        RefreshToken refreshToken = await _authenticationProvider.RefreshRefreshTokenAsync(user);
        ArgumentNullException.ThrowIfNull(user.RefreshTokenId);
        await _userRepository.SaveChangesAsync();

        JwtSecurityToken accessToken = _authenticationProvider.CreateAccessToken(user);

        return Ok(accessToken.ToResponseModel(refreshToken));
    }
}
