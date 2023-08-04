namespace foodremedy.api.Models.Responses;

public record IngredientSummary(string Id, Measurement Metadata, NutritionalInformationSummary NutritionalInformation);
