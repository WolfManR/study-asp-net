using Microsoft.AspNetCore.Identity;

namespace Bank.Authentication.Data;

public sealed class AuthenticationUser : IdentityUser
{
    public string? Token { get; set; }
    public DateTime TokenExpires { get; set; }
}