namespace Bank.Accounts.Data;

public sealed class AccountEntity
{
    public int Id { get; init; }
    public string IdentityId { get; set; }
    public string Holder { get; set; }
}