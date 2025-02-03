namespace HttpClientDelegatingHandler.Configurations;

internal class JokeApiOptions
{
    public required Uri BaseUri { get; set; }
    public required string RandomJokePath { get; set; }
    public int RetryAttempt { get; set; }
}