﻿using System.IdentityModel.Tokens.Jwt;
using foodremedy.api.Models.Responses;
using foodremedy.database.Models;
using Ingredient = foodremedy.database.Models.Ingredient;
using Tag = foodremedy.api.Models.Responses.Tag;
using TagCategory = foodremedy.api.Models.Responses.TagCategory;
using User = foodremedy.database.Models.User;

namespace foodremedy.api.Extensions;

public static class DatabaseModelExtensions
{
    public static Tag ToResponseModel(this database.Models.Tag tag)
    {
        return new Tag(tag.Id, tag.Name, tag.TagCategory.Name);
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
        var tags = ingredient.Tags.GroupBy(p => p.TagCategory);
        var tagDictionary = new Dictionary<string, IEnumerable<string>>();

        foreach (var grouping in tags)
        {
            tagDictionary.Add(grouping.Key.Name, grouping.Select(p => p.Name).ToList());
        }
        
        return new Models.Responses.Ingredient(
            ingredient.Id, 
            ingredient.Description,
            tagDictionary);
    }

    public static PaginatedResponse<TResponse> ToResponseModel<TResponse, TDbModel>(
        this PaginatedResult<TDbModel> result, Func<TDbModel, TResponse> map)
        where TResponse : class where TDbModel : class
    {
        return new PaginatedResponse<TResponse>(result.Total, result.Count, result.Results.Select(map));
    }

    public static TagCategory ToResponseModel(this database.Models.TagCategory tagCategory)
    {
        return new TagCategory(tagCategory.Id, tagCategory.Name);
    }

    public static Models.Responses.User ToResponseModel(this User user)
    {
        return new Models.Responses.User(
            user.Id,
            user.Email
        );
    }
}
