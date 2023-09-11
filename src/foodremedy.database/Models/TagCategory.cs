namespace foodremedy.database.Models;

public class TagCategory
{
    public TagCategory(string name)
    {
        Name = name;
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
}
