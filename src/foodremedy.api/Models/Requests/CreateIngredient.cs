using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace foodremedy.api.Models.Requests;

public record CreateIngredient(
    [Required(AllowEmptyStrings = false)] string Description,
    Dictionary<string, IEnumerable<string>>? Tags
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Tags.IsNullOrEmpty())
            yield break;

        foreach (KeyValuePair<string, IEnumerable<string>> tagGroup in Tags!)
            if (tagGroup.Value.IsNullOrEmpty())
                yield return new ValidationResult("Included tag collections must not be empty", new[] { nameof(Tags) });
    }
}
