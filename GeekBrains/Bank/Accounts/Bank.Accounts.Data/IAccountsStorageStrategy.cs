namespace Bank.Accounts.Data;

public interface IAccountsStorageStrategy
{
    ValueTask<int> Create(Account account);
    ValueTask Delete(int id);
    ValueTask<Account> Get(int id);
    Task<IEnumerable<Account>> Get();
}