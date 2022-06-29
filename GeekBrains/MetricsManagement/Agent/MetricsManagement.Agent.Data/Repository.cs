namespace MetricsManagement.Agent.Data;

public class Repository
{
    private readonly IStorageStrategy _storageStrategy;

    public Repository(IStorageStrategy storageStrategy)
    {
        _storageStrategy = storageStrategy;
    }

    public string TableName
    {
        set => _storageStrategy.TableName = value;
    }

    public void Create(int value, long time)
    {
        _storageStrategy.Create(value, time);
    }

    public IEnumerable<Metric> Get(DateTimeOffset from, DateTimeOffset to)
    {
        var fromSeconds = from.ToUnixTimeSeconds();
        var toSeconds = to.ToUnixTimeSeconds();

        if (fromSeconds == toSeconds)
        {
            return _storageStrategy.Get(fromSeconds);
        }

        var (min, max) = fromSeconds > toSeconds
            ? (toSeconds, fromSeconds)
            : (fromSeconds, toSeconds);

        return _storageStrategy.Get(min, max);
    }
}