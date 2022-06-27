namespace MetricsManagement.Manager.Data;

public class MetricsRepository
{
    private static List<Metric> _metrics = new();
    public string TableName { get; set; }

    public void Create(int agentId, int value, long time)
    {
        _metrics.Add(new() {AgentId = agentId, Value = value, Time = time });
    }

    public IEnumerable<Metric> Get(int agentId, DateTimeOffset from, DateTimeOffset to)
    {
        var fromSeconds = from.ToUnixTimeSeconds();
        var toSeconds = to.ToUnixTimeSeconds();

        if (fromSeconds == toSeconds)
        {
            return _metrics.Where(e => e.AgentId == agentId && e.Time == fromSeconds);
        }

        var (min, max) = fromSeconds > toSeconds
            ? (toSeconds, fromSeconds)
            : (fromSeconds, toSeconds);

        return _metrics.Where(e => e.AgentId == agentId && e.Time >= min && e.Time < max);
    }

    public DateTimeOffset GetAgentLastMetricDate(int agentId)
    {
        var agentMetrics = _metrics.Where(e => e.AgentId == agentId).ToArray();
        if (agentMetrics.Length == 0) return DateTimeOffset.MinValue;
        var time = agentMetrics.Max(e => e.Time);
        return DateTimeOffset.FromUnixTimeSeconds(time);
    }
}