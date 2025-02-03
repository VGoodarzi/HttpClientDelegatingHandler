namespace HttpClientDelegatingHandler.Models;

internal class RandomJokeModel
{
    public required string Type { get; init; }
    public required string Setup { get; init; }
    public required string Punchline { get; init; }
    public long Id { get; init; }
}