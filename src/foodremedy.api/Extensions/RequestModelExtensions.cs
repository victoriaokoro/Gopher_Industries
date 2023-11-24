using foodremedy.api.Models.Requests;
using foodremedy.api.Utils;
using foodremedy.database.Models;

namespace foodremedy.api.Extensions;

public static class RequestModelExtensions
{
    public static Tag ToDbModel(this CreateTag createTag, TagCategory tagCategory)
    {
        return new Tag
        {
            Name = createTag.Name,
            TagCategory = tagCategory
        };
    }

    public static User ToDbModel(this RegisterUser registerUser)
    {
        string salt = StringHasher.GenerateSalt();
        return new User(registerUser.Email, StringHasher.Hash(registerUser.Password, salt), salt);
    }

    public static Ingredient ToDbModel(this CreateIngredient createIngredient, List<TagCategory>? tagCategories = null)
    {
        if (createIngredient.Tags.IsNullOrEmpty() || tagCategories.IsNullOrEmpty())
            return new Ingredient(createIngredient.Description)
            {
                Tags = Array.Empty<Tag>()
            };
        
        var tags = new List<Tag>();
        
        foreach (KeyValuePair<string, IEnumerable<string>> tagGroup in createIngredient.Tags!)
        {
            TagCategory? category = tagCategories!
                .SingleOrDefault(p => p.Name.Equals(tagGroup.Key, StringComparison.InvariantCultureIgnoreCase));

            ArgumentNullException.ThrowIfNull(category);

            tags.AddRange(tagGroup.Value.Select(p =>
            {
                Tag? dbTag = category
                    .Tags
                    .SingleOrDefault(q => q.Name.Equals(p, StringComparison.InvariantCultureIgnoreCase));

                ArgumentNullException.ThrowIfNull(dbTag);

                return dbTag;
            }));
        }

        return new Ingredient(createIngredient.Description)
        {
            Tags = tags
        };
    }

    public static TagCategory ToDbModel(this CreateTagCategory createTagCategory)
    {
        return new TagCategory
        {
            Name = createTagCategory.Name,
            Tags = Array.Empty<Tag>()
        };
    }
}
