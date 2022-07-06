using Bank.Accounts.Data;
using Dapper;

namespace Banks.Accounts.Data.Dapper;

public sealed class DapperAccountsStorageStrategy : IAccountsStorageStrategy
{
    private readonly IDbConnector _connector;

    private const string TableName = "\"Accounts\"";

    private const string sqlGetAll = $"SELECT * FROM {TableName}";
    private const string sqlGetById = $"SELECT * FROM {TableName} WHERE \"Id\" = @Id";
    private const string sqlInsert = $"INSERT INTO {TableName} (\"Holder\") Values (@Holder) RETURNING \"Id\";";
    private const string sqlDelete = $"DELETE FROM {TableName} WHERE \"Id\" = @Id;";

    public DapperAccountsStorageStrategy(IDbConnector connector)
    {
        _connector = connector;
    }

    public async ValueTask<int> Create(Account account)
    {
        await using var connection = _connector.Create();
        var returnedId = await connection.ExecuteScalarAsync<int>(sqlInsert, new { account.Holder });
        return returnedId;
    }

    public async ValueTask Delete(int id)
    {
        await using var connection = _connector.Create();
        await connection.ExecuteAsync(sqlDelete, new { Id = id });
    }

    public async ValueTask<Account> Get(int id)
    {
        await using var connection = _connector.Create();
        var entity = await connection.QueryFirstOrDefaultAsync<AccountEntity>(sqlGetById, new { Id = id });
        if (entity is null) return null;
        return new Account() { Holder = entity.Holder };
    }

    public async Task<IEnumerable<Account>> Get()
    {
        await using var connection = _connector.Create();
        var data = await connection.QueryAsync<AccountEntity>(sqlGetAll);
        if (data is null) return Array.Empty<Account>();
        var result = data.Select(e => new Account() { Holder = e.Holder }).ToArray();
        return result;
    }
}