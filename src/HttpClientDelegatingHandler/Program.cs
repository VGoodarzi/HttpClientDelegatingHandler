using HttpClientDelegatingHandler.Configurations;
using HttpClientDelegatingHandler.HttpClients;
using HttpClientDelegatingHandler.HttpClients.MessageHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
services.AddOptions<JokeApiOptions>()
    .Configure(option =>
    {
        option.BaseUri = new Uri("https://official-joke-api.appspot.com/");
        option.RandomJokePath = "/random_joke";
        option.RetryAttempt = 3;
    });

services.AddSingleton<JokeApiClient>();
services.AddSingleton<LoggingDelegatingHandler>();
services.AddSingleton<RetryDelegatingHandler>();
services.AddHttpClient<JokeApiClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<JokeApiOptions>>();

    client.BaseAddress = options.Value.BaseUri;
})
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddHttpMessageHandler<RetryDelegatingHandler>()
  .RemoveAllLoggers();

await using var sp = services.BuildServiceProvider();
var jokeApiClient = sp.GetRequiredService<JokeApiClient>();
var logger = sp.GetRequiredService<ILogger<Program>>();

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, eventArgs) =>
{
    Console.WriteLine("Cancel event triggered");
    cts.Cancel();
    eventArgs.Cancel = true;
};

try
{
    while (cts.IsCancellationRequested is false)
    {

        var joke = await jokeApiClient.GetRandomJoke(cts.Token);

        logger.LogInformation("Joke:'{Id}', Type: '{Type}', Setup: '{Setup}', Punchline: '{Punchline}'",
            joke.Id, joke.Type, joke.Setup, joke.Punchline);

        await Task.Delay(1000, cts.Token);
    }
}
catch (TaskCanceledException)
{
  
}
catch (Exception ex)
{
    logger.LogCritical(ex, "An error was thrown");
    cts.Cancel();
}



