using MetricsManagement.Manager.Client;
using MetricsManagement.Manager.Data;

using Quartz;

namespace MetricsManagement.Manager.Jobs;

[DisallowConcurrentExecution]
public class ProcessorTimeJob : IJob
{
    private readonly MetricsClient _client;
    private readonly AgentsRepository _agentsRepository;
    private readonly MetricsRepository _metricsRepository;

    private const string MetricEndpoint = "windows/processor-time/total";

    public ProcessorTimeJob(MetricsClient client, AgentsRepository agentsRepository, MetricsRepository metricsRepository)
    {
        _client = client;
        _agentsRepository = agentsRepository;
        _metricsRepository = metricsRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var agents = _agentsRepository.Get().Where(a => a.IsEnabled);
        var metrics = await Task.WhenAll(agents.Select(a=> GetAgentMetrics(a.Uri, a.Id)));
        AddNewMetrics(metrics.SelectMany(e => e));
    }

    private async Task<IEnumerable<Metric>> GetAgentMetrics(string uri, int agentId)
    {
        var response = await _client.GetMetrics(uri, MetricEndpoint, _metricsRepository.GetAgentLastMetricDate(agentId), DateTimeOffset.UtcNow);
        return response.Select(m=> new Metric{AgentId = agentId, Time = m.Time, Value = m.Value}).ToArray();
    }

    private void AddNewMetrics(IEnumerable<Metric> metrics)
    {
        foreach (var metric in metrics)
        {
            _metricsRepository.Create(metric.AgentId, metric.Value, metric.Time);
        }
    }
}