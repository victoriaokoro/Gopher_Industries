using System.ComponentModel.DataAnnotations;
using foodremedy.database.Models;

namespace foodremedy.api.Models.Requests;

public record CreateTag(
    [Required(AllowEmptyStrings = false)] string Description,
    [Required(AllowEmptyStrings = false)] string TagType
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Enum.TryParse(TagType, true, out TagType _))
            yield return new ValidationResult($"Invalid tag type {TagType}.",
                new[] { nameof(TagType) });
    }
}
