using Bank.Accounts.Data;
using Npgsql;

namespace Bank.AccountsServer.Database;

public class PostgresDbInitializer
{
    private readonly ConnectionString _connectionString;

    public PostgresDbInitializer(ConnectionString connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeAccountsDatabase()
    {
        var connectionString = _connectionString.BuildWithoutDatabase();
        var database = _connectionString.Database;

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            connection.ChangeDatabase(database);
        }
        catch (PostgresException e)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE \"{database}\"";
            command.ExecuteNonQuery();
            connection.ChangeDatabase(database);
        }
    }
}