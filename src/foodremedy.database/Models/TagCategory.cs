namespace foodremedy.database.Models;

public class TagCategory
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public IEnumerable<Tag> Tags { get; init; }
}
