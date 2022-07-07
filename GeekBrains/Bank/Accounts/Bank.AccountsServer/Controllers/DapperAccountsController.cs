using App.Metrics;
using Bank.Accounts.Data;
using Bank.Accounts.Data.Dapper;

using Microsoft.AspNetCore.Mvc;

namespace Bank.AccountsServer.Controllers;

[Route("accounts/dapper")]
public sealed class DapperAccountsController : AccountsController
{
    public DapperAccountsController(
        AccountsRepository accountsRepository,
        DapperAccountsStorageStrategy storageStrategy,
        IMetrics metrics)
        : base(accountsRepository, metrics)
    {
        accountsRepository.StorageStrategy = storageStrategy;
    }

    protected override string MetricsContext => "DAPPER";
}