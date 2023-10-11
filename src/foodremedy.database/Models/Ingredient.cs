namespace foodremedy.database.Models;

public class Ingredient
{
    public Ingredient(string description)
    {
        Description = description;
        Tags = new List<Tag>();
    }

    public Guid Id { get; init; }
    public string Description { get; set; }
    public List<Tag> Tags { get; set; }
}
