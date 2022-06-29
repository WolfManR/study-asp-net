namespace MetricsManagement.Manager.Data;

public interface IMetricsStorageStrategy
{
    string? TableName { get; set; }

    void Create(int agentId, int value, long time);
    IEnumerable<Metric> Get(int agentId, long exactTime);
    IEnumerable<Metric> Get(int agentId, long from, long to);
    long GetAgentLastMetricDate(int agentId);
}