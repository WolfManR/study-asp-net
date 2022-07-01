using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TemplatesReporter.Mail.Core;

namespace TemplatesReporter.ApiClients;

public sealed class EmailSendService : BaseHttpClientService
{
    public EmailSendService(HttpClient client, ILogger<BaseHttpClientService> logger) : base(client, logger)
    {
    }

    public void SetToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task SendImmediately(EmailMessage email)
    {
        var json = JsonSerializer.Serialize(email);

        var response = await SendEmail(json, "emailsend/immediately");

        if (!response.IsSuccessStatusCode)
        {
            // TODO Fail notify
        }
    }

    public async Task ScheduleSend(EmailMessage email, DateTime date)
    {
        var json = JsonSerializer.Serialize(email);

        var response = await SendEmail(json, $"emailsend/scheduled/on/{date}");

        if (!response.IsSuccessStatusCode)
        {
            // TODO Fail notify
        }
    }

    private Task<HttpResponseMessage> SendEmail(string json, string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        return Send(request);
    }
}