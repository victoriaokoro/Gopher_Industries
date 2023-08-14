using foodremedy.api.Models.Requests;
using foodremedy.api.Utils;
using foodremedy.database.Models;

namespace foodremedy.api.Extensions;

public static class RequestModelExtensions
{
    public static Tag ToDbTag(this CreateTag createTag)
    {
        if (!Enum.TryParse(createTag.TagType, true, out TagType tagType))
            throw new ArgumentException($"Invalid tag type: {createTag.TagType}", nameof(createTag.TagType));

        return new Tag(createTag.Description, tagType);
    }

    public static User ToDbUser(this RegisterUser registerUser)
    {
        string salt = StringHasher.GenerateSalt();
        return new User(registerUser.Email, StringHasher.Hash(registerUser.Password, salt), salt);
    }
}
