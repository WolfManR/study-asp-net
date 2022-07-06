namespace Bank.Accounts.Data;

public class AccountsRepository
{
    private readonly IAccountsStorageStrategy _storage;

    public AccountsRepository(IAccountsStorageStrategy storage)
    {
        _storage = storage;
    }

    public async Task<int> Create(string holder)
    {
        var account = new Account() { Holder = holder };
        return await _storage.Create(account);
    }

    public async Task Delete(int id)
    {
        await _storage.Delete(id);
    }

    public Task<IEnumerable<Account>> Get()
    {
        return _storage.Get();
    }

    public async Task<Account> Get(int id)
    {
        return await _storage.Get(id);
    }
}