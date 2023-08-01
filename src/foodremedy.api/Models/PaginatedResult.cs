namespace foodremedy.api.Models;

public record PaginatedResult<T>(int Total, int Count, IEnumerable<T> Results) where T : class;
