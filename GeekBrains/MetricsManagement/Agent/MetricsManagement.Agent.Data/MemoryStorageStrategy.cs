namespace MetricsManagement.Agent.Data;

public class MemoryStorageStrategy : IStorageStrategy
{
    private static readonly List<Metric> Metrics = new();
    public string TableName { get; set; }

    public void Create(int value, long time)
    {
        Metrics.Add(new() { Value = value, Time = time });
    }

    public IEnumerable<Metric> Get(long exactTime)
    {
        return Metrics.Where(e => e.Time == exactTime);
    }

    public IEnumerable<Metric> Get(long from, long to)
    {
        return Metrics.Where(e => e.Time >= from && e.Time < to);
    }
}