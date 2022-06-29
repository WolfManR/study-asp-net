namespace MetricsManagement.Agent.Data;

public interface IStorageStrategy
{
    string TableName { set; }

    void Create(int value, long time);
    IEnumerable<Metric> Get(long exactTime);
    IEnumerable<Metric> Get(long from, long to);
}