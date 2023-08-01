namespace foodremedy.api.Models;

public record NutritionalInformationSummary
{
    public Measurement Energy { get; init; }
    public Measurement Carbohydrates { get; init; }
    public Measurement Fat { get; init; }
    public Measurement Protein { get; init; }
}
