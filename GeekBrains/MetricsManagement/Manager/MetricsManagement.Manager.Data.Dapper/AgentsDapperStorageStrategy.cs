using Dapper;

namespace MetricsManagement.Manager.Data.Dapper;

public class AgentsDapperStorageStrategy : IAgentsStorageStrategy
{
    private readonly IDbConnector _connector;
    private readonly string _tableName;

    public AgentsDapperStorageStrategy(IDbConnector connector)
    {
        _connector = connector;
        _tableName = MetricsTables.Agents;
    }

    public int Register(string uri)
    {
        using var connection = _connector.Create();
        var count = connection.ExecuteScalar<int>($"SELECT Count(*) FROM {_tableName} WHERE uri=@uri;", new { uri });
        if (count > 0) return default;

        var result = connection.Execute(
            $"INSERT INTO {_tableName}(uri,isenabled) VALUES (@uri,@isenabled);",
            new { uri, isenabled = true }
        );
        if (result == 0) return default;

        var id = connection.ExecuteScalar<int>($"SELECT (id) FROM {_tableName} WHERE uri=@uri;", new { uri });
        return id;
    }

    public void Enable(int id)
    {
        using var connection = _connector.Create();

        var count = connection.ExecuteScalar<int>(
            $"SELECT Count(*) FROM {_tableName} WHERE id=@id",
            new { id }
        );
        if (count <= 0) return;

        connection.Execute(
            $"UPDATE {_tableName} SET isenabled=@isenabled where id=@id;",
            new { isenabled = true, id });
    }

    public void Disable(int id)
    {
        using var connection = _connector.Create();

        var count = connection.ExecuteScalar<int>(
            $"SELECT Count(*) FROM {_tableName} WHERE id=@id",
            new { id }
        );
        if (count <= 0) return;

        connection.Execute(
            $"UPDATE {_tableName} SET isenabled=@isenabled where id=@id;",
            new { isenabled = false, id });
    }

    public IEnumerable<Agent> Get()
    {
        using var connection = _connector.Create();
        return connection.Query<Agent>($"SELECT * FROM {_tableName}").ToList();
    }
}