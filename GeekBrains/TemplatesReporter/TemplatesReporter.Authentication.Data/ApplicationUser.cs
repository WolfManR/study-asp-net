using Microsoft.AspNetCore.Identity;

namespace TemplatesReporter.Authentication.Data;

public sealed class ApplicationUser : IdentityUser
{
    public string Token { get; init; }
}