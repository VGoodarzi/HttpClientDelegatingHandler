using HttpClientDelegatingHandler.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttpClientDelegatingHandler.HttpClients.MessageHandlers;

internal class RetryDelegatingHandler(
    IOptions<JokeApiOptions> options,
    ILogger<RetryDelegatingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        var counter = 0;
        do
        {
            response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
                break;

            logger.LogWarning("Response has failed");

            await Task.Delay(100, cancellationToken);

        } while (counter++ < options.Value.RetryAttempt);


        return response;
    }
}