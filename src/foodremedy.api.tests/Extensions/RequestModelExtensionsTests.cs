using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using foodremedy.api.Extensions;
using foodremedy.database.Models;

namespace foodremedy.api.tests.Extensions;

public class RequestModelExtensionsTests
{
    [Fact]
    public void Should_map_between_DbTag_and_ResponseTag()
    {
        var dbTag = new Tag("Some description", Guid.NewGuid());
        var result = dbTag.ToResponseModel();

        result.Name.Should().Be(dbTag.Name);
        result.TagCategory.Should().Be(dbTag.TagCategory);
    }

    [Fact]
    public void Should_map_between_JwtSecurityToken_and_AccessTokenCreated()
    {
        var refreshToken = new RefreshToken("some refresh token");
        var jwt = new JwtSecurityToken(expires: DateTime.UtcNow.AddHours(1));

        var result = jwt.ToResponseModel(refreshToken);

        result.RefreshToken.Should().Be(refreshToken.Token);
        result.TokenType.Should().Be("Bearer");
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.ExpiresIn.Should().BeCloseTo((int)TimeSpan.FromHours(1).TotalSeconds, 10);
    }
}
