using static Dapper.SqlMapper;

namespace MetricsManagement.Agent.Data.Dapper;

public class DapperStorageStrategy : IStorageStrategy
{
    private readonly IDbConnector _connector;

    public DapperStorageStrategy(IDbConnector connector)
    {
        _connector = connector;
    }

    public string? TableName { get; set; }

    public void Create(int value, long time)
    {
        if (TableName is null) throw new ArgumentNullException(nameof(TableName));

        using var connection = _connector.Create();
        connection.Execute(
            $"INSERT INTO {TableName}(value,time) VALUES (@value,@time);",
            new { value, time }
        );
    }

    public IEnumerable<Metric> Get(long exactTime)
    {
        if (TableName is null) throw new ArgumentNullException(nameof(TableName));

        using var connection = _connector.Create();
        return connection.Query<Metric>(
                $"SELECT * FROM {TableName} WHERE (time = @exactTime);",
                new { exactTime })
            .ToList();
    }

    public IEnumerable<Metric> Get(long from, long to)
    {
        if (TableName is null) throw new ArgumentNullException(nameof(TableName));

        using var connection = _connector.Create();
        return connection.Query<Metric>(
                $"SELECT * FROM {TableName} WHERE (time > @from) and (time < @to);",
                new { from, to })
            .ToList();
    }
}