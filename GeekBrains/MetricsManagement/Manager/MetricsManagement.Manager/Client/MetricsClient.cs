using System.Text.Json;

namespace MetricsManagement.Manager.Client;

public class MetricsClient
{
    private readonly HttpClient _client;

    public MetricsClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<ICollection<AgentMetric>> GetMetrics(string agentUri, string metricUri, DateTimeOffset from, DateTimeOffset to)
    {
        using var response = await _client.GetAsync($"{agentUri}/{metricUri}", HttpCompletionOption.ResponseHeadersRead);
        if (!response.IsSuccessStatusCode) return Array.Empty<AgentMetric>();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<ICollection<AgentMetric>>(stream);
        return result ?? Array.Empty<AgentMetric>();
    }
}