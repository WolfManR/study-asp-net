using System.Text.Json;
using MetricsManagement.Manager.Data;

namespace MetricsManagement.Manager.Client;

public class MetricsClient
{
    private readonly HttpClient _client;

    public MetricsClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<ICollection<Metric>> GetMetrics(string agentUri, string metricUri, DateTimeOffset from, DateTimeOffset to)
    {
        using var response = await _client.GetAsync($"{agentUri}/{metricUri}", HttpCompletionOption.ResponseHeadersRead);
        if (!response.IsSuccessStatusCode) return Array.Empty<Metric>();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<ICollection<Metric>>(stream);
        return result ?? Array.Empty<Metric>();
    }
}