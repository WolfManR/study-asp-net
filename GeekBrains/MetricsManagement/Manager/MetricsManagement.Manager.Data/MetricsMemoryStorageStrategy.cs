namespace MetricsManagement.Manager.Data;

public class MetricsMemoryStorageStrategy : IMetricsStorageStrategy
{
    private static readonly List<Metric> Metrics = new();
    public string? TableName { get; set; }

    public void Create(int agentId, int value, long time)
    {
        Metrics.Add(new() { AgentId = agentId, Value = value, Time = time });
    }

    public IEnumerable<Metric> Get(int agentId, long exactTime)
    {
        return Metrics.Where(e => e.AgentId == agentId && e.Time == exactTime);
    }
    public IEnumerable<Metric> Get(int agentId, long from, long to)
    {
        return Metrics.Where(e => e.AgentId == agentId && e.Time >= from && e.Time < to);
    }

    public long GetAgentLastMetricDate(int agentId)
    {
        var agentMetrics = Metrics.Where(e => e.AgentId == agentId).ToArray();
        if (agentMetrics.Length == 0) return default;
        var time = agentMetrics.Max(e => e.Time);
        return time;
    }
}