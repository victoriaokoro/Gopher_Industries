namespace foodremedy.api.Models.Responses;

public record NutritionalInformation(
    Measurement EnergyWithDietaryFibre,
    Measurement Moisture,
    Measurement Protein,
    Measurement Nitrogen,
    Measurement Fat,
    Measurement Ash,
    Measurement DietaryFibre,
    Measurement Alcohol,
    Measurement Fructose,
    Measurement Glucose,
    Measurement Sucrose,
    Measurement Maltose,
    Measurement Lactose,
    Measurement Galactose,
    Measurement TotalSugars,
    Measurement AddedSugars,
    Measurement FreeSugars,
    Measurement Starch,
    Measurement ResistantStarch,
    Measurement CarbohydratesWithSugarAlcohols,
    Measurement CarbohydratesWithoutSugarAlcohols
);
