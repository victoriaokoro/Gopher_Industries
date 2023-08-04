using System.ComponentModel.DataAnnotations;

namespace foodremedy.api.Models.Requests;

public record AttemptLogin(
    [Required(AllowEmptyStrings = false)] [EmailAddress] string Email,
    [Required(AllowEmptyStrings = false)] string Password
);
