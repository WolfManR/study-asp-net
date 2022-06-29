namespace MetricsManagement.Manager.Data;

public class MetricsRepository
{
    private readonly IMetricsStorageStrategy _storageStrategy;

    public MetricsRepository(IMetricsStorageStrategy storageStrategy)
    {
        _storageStrategy = storageStrategy;
    }

    public string? TableName { set => _storageStrategy.TableName = value; }

    public void Create(int agentId, int value, long time)
    {
        _storageStrategy.Create(agentId, value, time);
    }

    public IEnumerable<Metric> Get(int agentId, DateTimeOffset from, DateTimeOffset to)
    {
        var fromSeconds = from.ToUnixTimeSeconds();
        var toSeconds = to.ToUnixTimeSeconds();

        if (fromSeconds == toSeconds)
        {
            return _storageStrategy.Get(agentId, fromSeconds);
        }

        var (min, max) = fromSeconds > toSeconds
            ? (toSeconds, fromSeconds)
            : (fromSeconds, toSeconds);

        return _storageStrategy.Get(agentId, min, max);
    }

    public DateTimeOffset GetAgentLastMetricDate(int agentId)
    {
        var time = _storageStrategy.GetAgentLastMetricDate(agentId);
        return time > 0
            ? DateTimeOffset.FromUnixTimeSeconds(time)
            : DateTimeOffset.MinValue;
    }
}