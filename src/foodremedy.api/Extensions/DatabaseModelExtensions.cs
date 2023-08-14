using System.IdentityModel.Tokens.Jwt;
using foodremedy.api.Models.Responses;
using foodremedy.database.Models;
using Tag = foodremedy.api.Models.Responses.Tag;

namespace foodremedy.api.Extensions;

public static class DatabaseModelExtensions
{
    public static Tag ToResponseModel(this database.Models.Tag tag)
    {
        return new Tag(tag.Id, tag.Description, tag.TagType.ToString());
    }

    public static AccessTokenCreated ToResponseModel(this JwtSecurityToken accessToken, RefreshToken refreshToken)
    {
        return new AccessTokenCreated(
            "Bearer",
            new JwtSecurityTokenHandler().WriteToken(accessToken),
            (int)(accessToken.ValidTo - DateTime.UtcNow).TotalSeconds,
            refreshToken.Token
        );
    }
}
