using System.ComponentModel.DataAnnotations;

namespace foodremedy.api.Models.Requests;

public record RefreshAccessToken(
    [Required(AllowEmptyStrings = false)] string RefreshToken
);
