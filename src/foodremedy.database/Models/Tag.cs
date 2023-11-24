namespace foodremedy.database.Models;

public class Tag
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public TagCategory TagCategory { get; init; }
}
