using System.Data.Common;

namespace Banks.Accounts.Data.Dapper;

public interface IDbConnector
{
    DbConnection Create();
}