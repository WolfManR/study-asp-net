using System.Data.Common;

namespace MetricsManagement.Manager.Data.Dapper;

public interface IDbConnector
{
    public DbConnection Create();
}