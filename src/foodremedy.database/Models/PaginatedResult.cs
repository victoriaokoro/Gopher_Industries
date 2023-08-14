namespace foodremedy.database.Models;

public record PaginatedResult<T>(int Count, int Total, List<T> Results) where T : class;
