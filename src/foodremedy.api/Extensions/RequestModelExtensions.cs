using foodremedy.api.Models.Requests;
using foodremedy.api.Utils;
using foodremedy.database.Models;

namespace foodremedy.api.Extensions;

public static class RequestModelExtensions
{
    public static Tag ToDbModel(this CreateTag createTag, Guid tagCategoryId)
    {
        return new Tag(createTag.Name, tagCategoryId);
    }

    public static User ToDbModel(this RegisterUser registerUser)
    {
        string salt = StringHasher.GenerateSalt();
        return new User(registerUser.Email, StringHasher.Hash(registerUser.Password, salt), salt);
    }

    public static Ingredient ToDbModel(this CreateIngredient createIngredient)
    {
        return new Ingredient(createIngredient.Description);
    }

    public static TagCategory ToDbModel(this CreateTagCategory createTagCategory)
    {
        return new TagCategory(createTagCategory.Name);
    }
}
