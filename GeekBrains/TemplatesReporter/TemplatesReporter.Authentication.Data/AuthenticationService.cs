using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TemplatesReporter.AuthenticationRules;

namespace TemplatesReporter.Authentication.Data;

public class AuthenticationService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthenticationService(IOptions<JwtSettings> jwtOptions, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _jwtSettings = jwtOptions.Value;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<string> Authenticate(string login, string password)
    {
        if (IsLoginAndPasswordNotValid(login, password)) return null;

        var result = await _signInManager.PasswordSignInAsync(login, password, false, false);
        if (!result.Succeeded) return null;

        var user = await _userManager.FindByEmailAsync(login);
        if (user is null) return null;
        var claims = await _userManager.GetClaimsAsync(user);

        return GenerateJwtToken(PrepareUserClaims(user.Id, claims));
    }

    public async Task<bool> RegisterUser(string login, string password)
    {
        if (IsLoginAndPasswordNotValid(login, password)) return false;

        var claims = new[] { new Claim(ClaimTypes.Role, "User") };

        var user = new ApplicationUser()
        {
            UserName = login,
            Email = login
        };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) return false;

        var assignClaimsResult = await _userManager.AddClaimsAsync(user, claims);
        return assignClaimsResult.Succeeded;
    }

    private IEnumerable<Claim> PrepareUserClaims(string userId, IList<Claim> claims)
    {
        claims.Add(new Claim(JwtRegisteredClaimNames.NameId, userId));
        var validAudiences = _jwtSettings.ValidAudience.Split(';', StringSplitOptions.TrimEntries);
        foreach (var audience in validAudiences)
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
        return claims;
    }

    private string GenerateJwtToken(IEnumerable<Claim> userClaims)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.SecureCode);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(userClaims),
            Issuer = _jwtSettings.ValidIssuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Expires = DateTime.Now.AddYears(2)
        };
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static bool IsLoginAndPasswordNotValid(string login, string password) => string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password);
}