namespace foodremedy.api.Models;

public record Ingredient
{
    public string Id { get; init; }
    public Measurement Metadata { get; init; }
    public NutritionalInformation NutritionalInformation { get; init; }
}
