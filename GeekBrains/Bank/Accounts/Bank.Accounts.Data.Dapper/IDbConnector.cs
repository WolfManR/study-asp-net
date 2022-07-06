using System.Data.Common;

namespace Bank.Accounts.Data.Dapper;

public interface IDbConnector
{
    DbConnection Create();
}