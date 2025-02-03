using Microsoft.Extensions.Logging;

namespace HttpClientDelegatingHandler.HttpClients.MessageHandlers;

internal class LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Http request for {RequestUri} has started", request.RequestUri);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
            logger.LogInformation("Http request for {RequestUri} has succeeded", 
                request.RequestUri);
        else
            logger.LogInformation("Http request for {RequestUri} has failed with Status : '{StatusCode}'", 
                request.RequestUri, response.StatusCode);

        return response;
    }
}