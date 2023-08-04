using System.ComponentModel.DataAnnotations;

namespace foodremedy.api.Configuration;

public record AuthenticationConfiguration
{
    public const string ConfigurationSection = "AuthenticationConfiguration";

    [Required(AllowEmptyStrings = false)] 
    public string SigningKey { get; init; } = null!;

    [Required(AllowEmptyStrings = false)] 
    public string Audience { get; init; } = null!;

    [Required(AllowEmptyStrings = false)] 
    public string Issuer { get; init; } = null!;

    [Required] 
    public int TokenTimeToLive { get; init; }
}
