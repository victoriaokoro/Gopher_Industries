namespace foodremedy.api.Models.Responses;

public sealed record User(
    Guid Id,
    string Email
);
