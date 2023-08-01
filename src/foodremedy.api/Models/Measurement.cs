namespace foodremedy.api.Models;

public record Measurement
{
    public int Amount { get; init; }
    public string Unit { get; init; }
};
