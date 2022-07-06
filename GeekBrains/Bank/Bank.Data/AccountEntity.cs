namespace Bank.Data;

public sealed class AccountEntity
{
    public int Id { get; init; }
    public string Number { get; set; }
    public string Holder { get; set; }
    public int ExpireMonth { get; set; }
    public int ExpireYear { get; set; }
}