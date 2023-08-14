namespace foodremedy.database.Models;

public class Ingredient
{
    public Ingredient(string description)
    {
        Description = description;
    }

    public Guid Id { get; init; }
    public string Description { get; set; }
    public List<Tag> SeasonTags { get; set; } = new();
    public List<Tag> ServingSizeTags { get; set; } = new();
}
