namespace Bank.Authentication.Data;

public sealed class TokenData
{
    public string Token { get; init; }
    public string RefreshToken { get; init; }
    public DateTime RefreshTokenExpirationDate { get; init; }
}