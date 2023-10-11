using System.ComponentModel.DataAnnotations;

namespace foodremedy.api.Models.Requests;

public record CreateTagCategory(
    [Required(AllowEmptyStrings = false)] string Name
);
