using Bank.Accounts.Data;

namespace Bank.AccountsServer.Database;

public static class ConfigurationExtensions
{
    public static ConnectionString GetConnectionStringBuilder(this IConfiguration configuration, string name)
    {
        return configuration.GetSection($"ConnectionStrings:{name}").Get<ConnectionString>();
    }
}