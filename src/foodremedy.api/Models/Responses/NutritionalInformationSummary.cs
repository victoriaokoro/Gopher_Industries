namespace foodremedy.api.Models.Responses;

public record NutritionalInformationSummary(
    Measurement Energy,
    Measurement Carbohydrates,
    Measurement Fat,
    Measurement Protein
);
