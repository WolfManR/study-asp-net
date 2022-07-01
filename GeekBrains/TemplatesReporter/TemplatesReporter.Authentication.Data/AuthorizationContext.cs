using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TemplatesReporter.Authentication.Data;

public class AuthorizationContext : IdentityDbContext<ApplicationUser>
{
    public AuthorizationContext(DbContextOptions<AuthorizationContext> options) : base(options)
    {

    }
}