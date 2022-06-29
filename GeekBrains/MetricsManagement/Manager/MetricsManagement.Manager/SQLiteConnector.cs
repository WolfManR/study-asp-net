using System.Data.Common;
using System.Data.SQLite;
using MetricsManagement.Manager.Data.Dapper;

public class SQLiteConnector : IDbConnector
{
    private readonly string _connectionString;

    public SQLiteConnector(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbConnection Create() => new SQLiteConnection(_connectionString);
}