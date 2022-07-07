using App.Metrics;
using Bank.Accounts.Data;
using Bank.Accounts.Data.EF;

using Microsoft.AspNetCore.Mvc;

namespace Bank.AccountsServer.Controllers;

[Route("accounts/ef")]
public sealed class EFAccountsController : AccountsController
{
    public EFAccountsController(
        AccountsRepository accountsRepository,
        EFAccountsStorageStrategy storageStrategy,
        IMetrics metrics) 
        : base(accountsRepository, metrics)
    {
        accountsRepository.StorageStrategy = storageStrategy;
    }

    protected override string MetricsContext => "EF";
}