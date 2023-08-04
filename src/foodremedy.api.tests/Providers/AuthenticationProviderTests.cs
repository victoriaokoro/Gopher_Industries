using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using foodremedy.api.Configuration;
using foodremedy.api.Providers;
using foodremedy.api.Repositories;
using foodremedy.api.Utils;
using foodremedy.database.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace foodremedy.api.tests.Providers;

public class AuthenticationProviderTests
{
    private readonly AuthenticationConfiguration _authenticationConfiguration = new()
    {
        SigningKey = "testKey",
        Audience = "testAudience",
        Issuer = "testIssuer",
        TokenTimeToLive = 3600
    };

    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository = new();
    private readonly AuthenticationProvider _sut;

    public AuthenticationProviderTests()
    {
        Mock<IOptions<AuthenticationConfiguration>> authenticationConfigurationOptions = new();

        authenticationConfigurationOptions.Setup(p => p.Value)
            .Returns(_authenticationConfiguration);

        _sut = new AuthenticationProvider(authenticationConfigurationOptions.Object, _refreshTokenRepository.Object);
    }

    [Fact]
    public void UserCanLogin_should_return_false_if_password_not_match()
    {
        var testUser = new User("testEmail", "testPassword", "testSalt");

        _sut.UserCanLogin(testUser, "wrongPassword").Should().BeFalse();
    }

    [Fact]
    public void UserCanLogin_should_return_true_if_password_match()
    {
        const string password = "testPassword";
        string salt = StringHasher.GenerateSalt();

        var testUser = new User("testEmail", StringHasher.Hash(password, salt), salt);

        _sut.UserCanLogin(testUser, password).Should().BeTrue();
    }

    [Fact]
    public void CreateAccessToken_should_return_bearer_token()
    {
        var apiUser = new User("", "", "")
        {
            Id = Guid.NewGuid()
        };

        JwtSecurityToken result = _sut.CreateAccessToken(apiUser);

        result.Should().BeOfType<JwtSecurityToken>();
        result.Should().NotBeNull();
        result.Issuer.Should().Be(_authenticationConfiguration.Issuer);
        result.Audiences.Should().ContainSingle();
        result.Audiences.Should().OnlyContain(p => p.Equals(_authenticationConfiguration.Audience));
        result.ValidFrom.Should().BeCloseTo(DateTimeOffset.UtcNow.DateTime, TimeSpan.FromSeconds(10));
        result.ValidTo.Should()
            .BeCloseTo(DateTimeOffset.UtcNow.DateTime.AddSeconds(_authenticationConfiguration.TokenTimeToLive),
                TimeSpan.FromSeconds(10));
        result.Claims.Select(c => c.Type).Should().Contain(new[]
        {
            JwtRegisteredClaimNames.Iat,
            JwtRegisteredClaimNames.Nbf,
            JwtRegisteredClaimNames.Exp
        });
        result.Claims.Should().ContainSingle(p => p.Type.Equals(JwtRegisteredClaimNames.Sub)).Which.Value.Should()
            .Be(apiUser.Id.ToString());
        result.Claims.Should().ContainSingle(p => p.Type.Equals(JwtRegisteredClaimNames.Jti)).Which.Value.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task RefreshRefreshToken_should_set_new_refreshtoken_if_none_existing()
    {
        var user = new User("", "", "")
        {
            RefreshTokenId = null
        };

        await _sut.RefreshRefreshTokenAsync(user);

        user.RefreshTokenId.Should().NotBeNull();
        _refreshTokenRepository.Verify(p =>
                p.Add(It.Is<RefreshToken>(q => !string.IsNullOrWhiteSpace(q.Token) && q.Id.Equals(Guid.Empty))),
            Times.Once);
    }

    [Fact]
    public async Task RefreshRefreshToken_should_set_existing_refreshtoken_if_existing()
    {
        const string oldToken = "oldToken";
        var existingRefreshToken = new RefreshToken(oldToken)
        {
            Id = Guid.NewGuid()
        };
        var user = new User("", "", "")
        {
            RefreshTokenId = existingRefreshToken.Id
        };

        _refreshTokenRepository.Setup(p => p.GetByUserAsync(user)).ReturnsAsync(existingRefreshToken);

        RefreshToken result = await _sut.RefreshRefreshTokenAsync(user);

        user.RefreshTokenId.Should().Be(existingRefreshToken.Id);
        result.Id.Should().Be(existingRefreshToken.Id);
        result.Token.Should().NotBeNullOrWhiteSpace().And.NotBe(oldToken);
    }

    [Fact]
    public async Task RefreshRefreshToken_should_throw_if_cant_find_user_token()
    {
        Func<Task> act = async () =>
        {
            await _sut.RefreshRefreshTokenAsync(new User("", "", "")
            {
                RefreshTokenId = Guid.NewGuid()
            });
        };

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("existingToken");
    }

    [Fact]
    public async Task RefreshRefreshToken_should_return_refreshtoken()
    {
        var user = new User("", "", "")
        {
            RefreshTokenId = null
        };

        RefreshToken result = await _sut.RefreshRefreshTokenAsync(user);

        result.Should().BeOfType<RefreshToken>();
        result.Token.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task RefreshTokenIsValid_should_return_false_if_token_not_found()
    {
        bool result = await _sut.RefreshTokenIsValidAsync(new User("", "", ""), "someToken");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task RefreshTokenIsValid_should_return_false_if_token_not_match()
    {
        var testUser = new User("", "", "");
        _refreshTokenRepository.Setup(p => p.GetByUserAsync(testUser)).ReturnsAsync(new RefreshToken("someToken"));

        bool result = await _sut.RefreshTokenIsValidAsync(testUser, "someOtherToken");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task RefreshTokenIsValid_should_return_true_if_token_match()
    {
        const string testToken = "testToken";
        var testUser = new User("", "", "");
        _refreshTokenRepository.Setup(p => p.GetByUserAsync(testUser)).ReturnsAsync(new RefreshToken(testToken));

        bool result = await _sut.RefreshTokenIsValidAsync(testUser, testToken);

        result.Should().BeTrue();
    }
}
