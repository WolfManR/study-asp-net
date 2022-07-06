using Microsoft.EntityFrameworkCore;

namespace Bank.Accounts.Data.EF;

public sealed class AccountsDbContext : DbContext
{
    public AccountsDbContext(DbContextOptions<AccountsDbContext> options) : base(options)
    {

    }

#nullable disable
    public DbSet<AccountEntity> Accounts { get; init; }
#nullable restore
}