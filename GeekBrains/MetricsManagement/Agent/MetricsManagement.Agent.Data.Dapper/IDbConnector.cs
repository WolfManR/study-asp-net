using System.Data.Common;

namespace MetricsManagement.Agent.Data.Dapper;

public interface IDbConnector
{
    public DbConnection Create();
}