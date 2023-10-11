namespace foodremedy.database.Models;

public record Tag(string Name, Guid TagCategoryId)
{
    public Guid Id { get; init; }
}
