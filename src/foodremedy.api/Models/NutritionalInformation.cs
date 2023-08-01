namespace foodremedy.api.Models;

public record NutritionalInformation
{
    public Measurement EnergyWithDietaryFibre { get; init; }
    public Measurement Moisture { get; init; }
    public Measurement Protein { get; init; }
    public Measurement Nitrogen { get; init; }
    public Measurement Fat { get; init; }
    public Measurement Ash { get; init; }
    public Measurement DietaryFibre { get; init; }
    public Measurement Alcohol { get; init; }
    public Measurement Fructose { get; init; }
    public Measurement Glucose { get; init; }
    public Measurement Sucrose { get; init; }
    public Measurement Maltose { get; init; }
    public Measurement Lactose { get; init; }
    public Measurement Galactose { get; init; }
    public Measurement TotalSugars { get; init; }
    public Measurement AddedSugars { get; init; }
    public Measurement FreeSugars { get; init; }
    public Measurement Starch { get; init; }
    public Measurement ResistantStarch { get; init; }
    public Measurement CarbohydratesWithSugarAlcohols { get; init; }
    public Measurement CarbohydratesWithoutSugarAlcohols { get; init; }
}
