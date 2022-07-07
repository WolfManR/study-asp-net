using Bank.Accounts.Data;
using Bank.Accounts.Data.Dapper;

using Microsoft.AspNetCore.Mvc;

namespace Bank.AccountsServer.Controllers;

[Route("accounts/dapper")]
public class DapperAccountsController : AccountsController
{
    public DapperAccountsController(AccountsRepository accountsRepository, DapperAccountsStorageStrategy storageStrategy) : base(accountsRepository)
    {
        accountsRepository.StorageStrategy = storageStrategy;
    }
}