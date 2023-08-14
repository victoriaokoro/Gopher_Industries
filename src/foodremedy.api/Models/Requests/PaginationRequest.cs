using System.ComponentModel.DataAnnotations;

namespace foodremedy.api.Models.Requests;

public record PaginationRequest(
    [Range(0, int.MaxValue)] int Skip = 0,
    [Range(0, int.MaxValue)] int Take = 20
);
