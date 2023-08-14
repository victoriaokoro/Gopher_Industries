using System.ComponentModel.DataAnnotations;

namespace foodremedy.api.Models.Requests;

public record CreateIngredient(
    [Required(AllowEmptyStrings = false)] string Description
);
