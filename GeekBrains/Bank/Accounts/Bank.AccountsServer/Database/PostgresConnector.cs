using Bank.Accounts.Data.Dapper;

using Npgsql;

using System.Data.Common;

namespace Bank.AccountsServer.Database;

public class PostgresConnector : IDbConnector
{
    private readonly string _connectionString;

    public PostgresConnector(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbConnection Create() => new NpgsqlConnection(_connectionString);
}