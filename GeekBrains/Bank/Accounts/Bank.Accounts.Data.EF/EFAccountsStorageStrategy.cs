using Microsoft.EntityFrameworkCore;

namespace Bank.Accounts.Data.EF;

public sealed class EFAccountsStorageStrategy : IAccountsStorageStrategy
{
    private readonly AccountsDbContext _context;

    public EFAccountsStorageStrategy(AccountsDbContext context)
    {
        _context = context;
    }

    public async ValueTask<int> Create(Account account)
    {
        AccountEntity entity = new() { Holder = account.Holder };
        _context.Accounts.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async ValueTask Delete(int id)
    {
        var entity = await _context.Accounts.FindAsync(id);
        if (entity is null) return;
        _context.Accounts.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async ValueTask<Account> Get(int id)
    {
        var entity = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null) return null;
        return new Account() { Holder = entity.Holder };
    }

    public async Task<IEnumerable<Account>> Get()
    {
        var result = await _context.Accounts.AsNoTracking().Select(e => new Account { Holder = e.Holder }).ToListAsync();
        return result;
    }
}