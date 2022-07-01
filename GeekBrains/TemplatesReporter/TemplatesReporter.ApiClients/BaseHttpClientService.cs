using Microsoft.Extensions.Logging;

namespace TemplatesReporter.ApiClients;

public abstract class BaseHttpClientService
{
    protected readonly HttpClient _client;
    private readonly ILogger<BaseHttpClientService> _logger;

    protected BaseHttpClientService(HttpClient client, ILogger<BaseHttpClientService> logger)
    {
        _client = client;
        _logger = logger;
    }

    protected async Task<HttpResponseMessage> Send(HttpRequestMessage requestMessage)
    {
        using (_logger.BeginScope("Send Request"))
        {
            try
            {
                var response = await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("{method} to {uri}", requestMessage.Method, requestMessage.RequestUri);
                    return response;
                }

                _logger.LogWarning("{statusCode} - {message}", response.StatusCode, response.ReasonPhrase);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail to send request");
                return null;
            }
        }
    }
}