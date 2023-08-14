using System.IdentityModel.Tokens.Jwt;
using foodremedy.api.Models.Responses;
using foodremedy.database.Models;
using Ingredient = foodremedy.database.Models.Ingredient;
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

    public static IngredientSummary ToSummaryResponseModel(this Ingredient ingredient)
    {
        return new IngredientSummary(ingredient.Id, ingredient.Description);
    }

    public static Models.Responses.Ingredient ToResponseModel(this Ingredient ingredient)
    {
        return new Models.Responses.Ingredient(ingredient.Id, ingredient.Description);
    }

    public static PaginatedResponse<TResponse> ToResponseModel<TResponse, TDbModel>(
        this PaginatedResult<TDbModel> result, Func<TDbModel, TResponse> map)
        where TResponse : class where TDbModel : class
    {
        return new PaginatedResponse<TResponse>(result.Total, result.Count, result.Results.Select(map));
    }
}
