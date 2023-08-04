using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using foodremedy.api.Controllers;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using foodremedy.api.Providers;
using foodremedy.api.Repositories;
using foodremedy.api.tests.Extensions;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace foodremedy.api.tests.Controllers;

public class AuthenticationControllerTests
{
    private readonly JwtSecurityToken _accessToken;
    private readonly Mock<IAuthenticationProvider> _authenticationProvider = new();
    private readonly AttemptLogin _loginRequest = new("testEmail", "testPassword");
    private readonly RefreshToken _refreshToken = new("testRefreshToken");
    private readonly AuthenticationController _sut;
    private readonly User _user;
    private readonly Mock<IUserRepository> _userRepository = new();

    public AuthenticationControllerTests()
    {
        _user = new User("", "", "")
        {
            Id = Guid.NewGuid()
        };
        _accessToken = new JwtSecurityToken(expires: DateTime.UtcNow.AddHours(1));
        _sut = new AuthenticationController(_userRepository.Object, _authenticationProvider.Object)
        {
            ControllerContext = GetFakeContext(new[] { new Claim(JwtRegisteredClaimNames.Sub, _user.Id.ToString()) })
        };

        _userRepository.Setup(p => p.GetUserByEmailAsync(_loginRequest.Email))
            .ReturnsAsync(_user);
        _userRepository.Setup(p => p.GetUserByIdAsync(_user.Id.ToString()))
            .ReturnsAsync(_user);
        _authenticationProvider.Setup(p => p.UserCanLogin(_user, _loginRequest.Password))
            .Returns(true);
        _authenticationProvider.Setup(p => p.RefreshRefreshTokenAsync(_user))
            .Callback<User>(p => p.RefreshTokenId = Guid.NewGuid())
            .ReturnsAsync(_refreshToken);
        _authenticationProvider.Setup(p => p.RefreshTokenIsValidAsync(_user, _refreshToken.Token))
            .ReturnsAsync(true);
        _authenticationProvider.Setup(p => p.CreateAccessToken(_user)).Returns(_accessToken);
    }

    private ControllerContext GetFakeContext(IEnumerable<Claim> claims)
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var context = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };
        return context;
    }

    [Fact]
    public async Task AttemptLogin_should_return_unauthorized_if_user_not_found()
    {
        _userRepository.Setup(p => p.GetUserByEmailAsync(_loginRequest.Email))
            .ReturnsAsync(null as User);

        ActionResult<AccessTokenCreated> response = await _sut.AttemptLogin(_loginRequest);

        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task AttemptLogin_should_return_unauthorized_if_wrong_credentials()
    {
        var testUser = new User("", "", "");

        _userRepository.Setup(p => p.GetUserByEmailAsync(_loginRequest.Email))
            .ReturnsAsync(testUser);
        _authenticationProvider.Setup(p => p.UserCanLogin(testUser, _loginRequest.Password))
            .Returns(false);

        ActionResult<AccessTokenCreated> result = await _sut.AttemptLogin(_loginRequest);

        _authenticationProvider.Verify(p => p.UserCanLogin(testUser, _loginRequest.Password), Times.Once);
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task AttemptLogin_should_throw_if_RefreshToken_not_set_after_refresh()
    {
        _authenticationProvider.Setup(p => p.RefreshRefreshTokenAsync(_user))
            .Callback<User>(p => p.RefreshTokenId = null)
            .ReturnsAsync(_refreshToken);

        Func<Task<ActionResult<AccessTokenCreated>>> act = async () => await _sut.AttemptLogin(_loginRequest);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("user.RefreshTokenId");
    }

    [Fact]
    public async Task AttemptLogin_should_return_AccessTokenCreated()
    {
        ActionResult<AccessTokenCreated> response = await _sut.AttemptLogin(_loginRequest);
        AccessTokenCreated? result = response.Unpack();

        response.Result.Should().BeOfType<OkObjectResult>();
        result.Should().NotBeNull();
        result!.TokenType.Should().Be("Bearer");
        result.RefreshToken.Should().Be(_refreshToken.Token);
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.ExpiresIn.Should().BeCloseTo((int)(_accessToken.ValidTo - DateTime.UtcNow).TotalSeconds, 10);
    }

    [Fact]
    public async Task RefreshAccessToken_should_return_unauthorized_if_userId_null()
    {
        var sut = new AuthenticationController(_userRepository.Object, _authenticationProvider.Object)
        {
            ControllerContext = GetFakeContext(Array.Empty<Claim>())
        };

        ActionResult<AccessTokenCreated> response = await sut.RefreshAccessToken(new RefreshAccessToken(""));

        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task RefreshAccessToken_should_return_unauthorized_if_user_not_found()
    {
        _userRepository.Setup(p => p.GetUserByIdAsync(_user.Id.ToString()))
            .ReturnsAsync(null as User);

        ActionResult<AccessTokenCreated> result = await _sut.RefreshAccessToken(new RefreshAccessToken(""));

        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task RefreshAccessToken_should_return_unauthorized_if_refreshtoken_invalid()
    {
        _authenticationProvider.Setup(p => p.RefreshTokenIsValidAsync(_user, _refreshToken.Token))
            .ReturnsAsync(false);

        ActionResult<AccessTokenCreated> result = await _sut.RefreshAccessToken(new RefreshAccessToken(""));

        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task RefreshAccessToken_should_throw_if_user_refresh_token_not_set_after_refresh()
    {
        _authenticationProvider.Setup(p => p.RefreshRefreshTokenAsync(_user))
            .Callback<User>(p => p.RefreshTokenId = null)
            .ReturnsAsync(_refreshToken);
        
        var act = async () => await _sut.RefreshAccessToken(new RefreshAccessToken(_refreshToken.Token));
        
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("user.RefreshTokenId");
    }

    [Fact]
    public async Task RefreshAccessToken_should_return_AccessTokenCreated()
    {
        ActionResult<AccessTokenCreated> response = await _sut.RefreshAccessToken(new RefreshAccessToken(_refreshToken.Token));
        AccessTokenCreated? result = response.Unpack();

        response.Result.Should().BeOfType<OkObjectResult>();
        result.Should().NotBeNull();
        result!.TokenType.Should().Be("Bearer");
        result.RefreshToken.Should().Be(_refreshToken.Token);
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.ExpiresIn.Should().BeCloseTo((int)(_accessToken.ValidTo - DateTime.UtcNow).TotalSeconds, 10);
    }
}
