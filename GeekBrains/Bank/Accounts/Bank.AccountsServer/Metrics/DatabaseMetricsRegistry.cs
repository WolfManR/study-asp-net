using App.Metrics.Counter;
using App.Metrics;

namespace Bank.AccountsServer.Metrics;

public class DatabaseMetricsRegistry
{
    public static CounterOptions CreatedAccountsCounter(string context) => new()
    {
        Name = "Created Accounts",
        Context = context,
        MeasurementUnit = Unit.Calls
    };
}