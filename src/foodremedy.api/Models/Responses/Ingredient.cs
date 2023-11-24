namespace foodremedy.api.Models.Responses;

public record Ingredient(
    Guid Id,
    string Description,
    Dictionary<string, IEnumerable<string>> Tags
);
