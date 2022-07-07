using Bank.Accounts.Data;
using Bank.Accounts.Data.EF;

using Microsoft.AspNetCore.Mvc;

namespace Bank.AccountsServer.Controllers;

[Route("accounts/ef")]
public class EFAccountsController : AccountsController
{
    public EFAccountsController(AccountsRepository accountsRepository, EFAccountsStorageStrategy storageStrategy) : base(accountsRepository)
    {
        accountsRepository.StorageStrategy = storageStrategy;
    }
}