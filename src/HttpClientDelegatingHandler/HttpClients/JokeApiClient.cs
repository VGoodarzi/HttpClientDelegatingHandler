using System.Text.Json;
using HttpClientDelegatingHandler.Configurations;
using HttpClientDelegatingHandler.Models;
using Microsoft.Extensions.Options;

namespace HttpClientDelegatingHandler.HttpClients;

internal class JokeApiClient(HttpClient client, IOptions<JokeApiOptions> options)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<RandomJokeModel> GetRandomJoke(CancellationToken cancellationToken)
    {
        var response = await client.GetAsync(options.Value.RandomJokePath, cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var joke = JsonSerializer.Deserialize<RandomJokeModel>(content, _jsonSerializerOptions);

        ArgumentNullException.ThrowIfNull(joke);

        return joke;
    }
}