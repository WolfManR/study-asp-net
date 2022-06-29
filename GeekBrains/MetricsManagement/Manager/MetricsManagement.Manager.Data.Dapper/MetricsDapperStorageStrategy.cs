using Dapper;

namespace MetricsManagement.Manager.Data.Dapper;

public class MetricsDapperStorageStrategy : IMetricsStorageStrategy
{
    private readonly IDbConnector _connector;

    public MetricsDapperStorageStrategy(IDbConnector connector)
    {
        _connector = connector;
    }

    public string? TableName { get; set; }

    public void Create(int agentId, int value, long time)
    {
        if (TableName is null) throw new ArgumentNullException(nameof(TableName));

        using var connection = _connector.Create();
        connection.Execute(
            $"INSERT INTO {TableName}(agentId,value,time) VALUES (@agentId,@value,@time);",
            new { agentId, value, time }
        );
    }

    public IEnumerable<Metric> Get(int agentId, long exactTime)
    {
        if (TableName is null) throw new ArgumentNullException(nameof(TableName));

        using var connection = _connector.Create();
        return connection.Query<Metric>(
                $"SELECT * FROM {TableName} WHERE (agentId = @agentId) and (time = @exactTime);",
                new { agentId, exactTime })
            .ToList();
    }

    public IEnumerable<Metric> Get(int agentId, long from, long to)
    {
        if (TableName is null) throw new ArgumentNullException(nameof(TableName));

        using var connection = _connector.Create();
        return connection.Query<Metric>(
                $"SELECT * FROM {TableName} WHERE (agentId = @agentId) and (time > @from) and (time < @to);",
                new { agentId, from, to })
            .ToList();
    }

    public long GetAgentLastMetricDate(int agentId)
    {
        using var connection = _connector.Create();
        var result = connection.ExecuteScalar<long>(
            $"select Max(time) from {TableName} where agentId = @agentId",
            new { agentId });

        return result;
    }
}