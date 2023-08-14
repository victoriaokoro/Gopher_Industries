namespace foodremedy.database.Models;

public enum TagType //TODO: Lock down tag categories
{
    NONE = 0,
    PLANT_COMPOUND,
    FOOD_PRESCRIPTION,
    TCM_ACTION,
    BENEFIT,
    SEASON,
    SERVING_SIZE,
    MINERAL,
    VITAMIN,
    PLANT_TYPE
}

public record Tag(string Description, TagType TagType)
{
    public Guid Id { get; init; }
}
