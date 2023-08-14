namespace foodremedy.api.Models.Responses;

public record PaginatedResponse<T>(int Total, int Count, IEnumerable<T> Results) where T : class;
