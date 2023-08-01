namespace foodremedy.api.Models;

public record IngredientSummary
{
    public string Id { get; init; }
    public Measurement Metadata { get; init; }
    public NutritionalInformationSummary NutritionalInformation { get; init; }
}
