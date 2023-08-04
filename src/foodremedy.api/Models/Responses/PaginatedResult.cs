namespace foodremedy.api.Models.Responses;

public record PaginatedResult<T>(int Total, int Count, IEnumerable<T> Results) where T : class;
