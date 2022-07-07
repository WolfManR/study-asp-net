namespace Bank.Accounts.Data;

public class AccountsRepository
{
    public IAccountsStorageStrategy? StorageStrategy { get; set; }

    public async Task<int> Create(string holder)
    {
        if(StorageStrategy is null) throw new ArgumentNullException(nameof(StorageStrategy));
        var account = new Account() { Holder = holder };
        return await StorageStrategy.Create(account);
    }

    public async Task Delete(int id)
    {
        if (StorageStrategy is null) throw new ArgumentNullException(nameof(StorageStrategy));
        await StorageStrategy.Delete(id);
    }

    public Task<IEnumerable<Account>> Get()
    {
        if (StorageStrategy is null) throw new ArgumentNullException(nameof(StorageStrategy));
        return StorageStrategy.Get();
    }

    public async Task<Account> Get(int id)
    {
        if (StorageStrategy is null) throw new ArgumentNullException(nameof(StorageStrategy));
        return await StorageStrategy.Get(id);
    }
}