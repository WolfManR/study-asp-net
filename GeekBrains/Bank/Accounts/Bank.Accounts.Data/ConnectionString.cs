using System.Text;

namespace Bank.Accounts.Data;

public sealed class ConnectionString
{
    public string Host { get; init; }
    public string Database { get; init; }
    public string UserId { get; init; }
    public string Password { get; init; }

    public string BuildWithoutDatabase() => Build(false);
    public string BuildWithDatabase() => Build(true);

    private string Build(bool includeDatabase)
    {
        StringBuilder sb = new();

        sb.Append($"Server={Host};");
        if (includeDatabase) sb.Append($"Database={Database};");
        sb.Append($"User Id={UserId};");
        sb.Append($"Password={Password};");

        return sb.ToString();
    }
}