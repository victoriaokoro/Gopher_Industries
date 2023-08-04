using System.ComponentModel.DataAnnotations;

namespace foodremedy.api.Models.Requests;

public record RegisterUser(
    [Required(AllowEmptyStrings = false)] [EmailAddress] string Email,
    [Required(AllowEmptyStrings = false)] string Password
);
