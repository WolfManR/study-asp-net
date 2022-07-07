using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bank.AuthenticationRules;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Thundire.Helpers;

namespace Bank.Authentication.Data;

public sealed class AuthenticationService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<AuthenticationUser> _userManager;
    private readonly SignInManager<AuthenticationUser> _signInManager;
    private readonly AuthorizationHelper _authorizationHelper;

    public AuthenticationService(
        IOptions<JwtSettings> jwtOptions,
        UserManager<AuthenticationUser> userManager,
        SignInManager<AuthenticationUser> signInManager,
        AuthorizationHelper authorizationHelper)
    {
        _jwtSettings = jwtOptions.Value;
        _userManager = userManager;
        _signInManager = signInManager;
        _authorizationHelper = authorizationHelper;
    }

    public async Task<Result<TokenData>> Authenticate(string login, string password)
    {
        if (IsLoginAndPasswordNotValid(login, password)) return Result<TokenData>.Fail();

        var result = await _signInManager.PasswordSignInAsync(login, password, false, false);
        if (!result.Succeeded) return Result<TokenData>.Fail();

        AuthenticationUser user = await _userManager.FindByEmailAsync(login);
        if (user is null) return Result<TokenData>.Fail();

        IList<Claim> userBaseClaims = await _userManager.GetClaimsAsync(user);
        IList<Claim> tokenClaims = PrepareUserClaims(user.Id, userBaseClaims);

        string token = GenerateJwtToken(tokenClaims, GenerateExpirationDate());

        DateTime refreshTokenExpirationDate = GenerateExpirationDate();
        string refreshToken = GenerateJwtToken(tokenClaims, refreshTokenExpirationDate);

        await UpdateUserToken(user, refreshToken, refreshTokenExpirationDate);

        TokenData tokenData = new()
        {
            Token = token,
            RefreshToken = refreshToken,
            RefreshTokenExpirationDate = refreshTokenExpirationDate
        };

        return Result<TokenData>.Ok(tokenData);
    }

    public async Task<bool> RegisterUser(string login, string password)
    {
        if (IsLoginAndPasswordNotValid(login, password)) return false;

        var claims = new[] { new Claim(ClaimTypes.Role, "User") };

        AuthenticationUser user = new()
        {
            UserName = login,
            Email = login
        };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) return false;

        var assignClaimsResult = await _userManager.AddClaimsAsync(user, claims);
        return assignClaimsResult.Succeeded;
    }

    public async Task<Result<(string, DateTime)>> RefreshToken(string oldToken)
    {
        if (!_authorizationHelper.TryGetUserIdFromToken(oldToken, out Guid userId))
        {
            return Result<(string, DateTime)>.Fail();
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result<(string, DateTime)>.Fail();

        if (string.CompareOrdinal(user.Token, oldToken) != 0 || IsExpired(user.TokenExpires))
        {
            return Result<(string, DateTime)>.Fail();
        }

        IList<Claim> userBaseClaims = await _userManager.GetClaimsAsync(user);
        DateTime tokenExpirationDate = GenerateExpirationDate();
        string newToken = GenerateJwtToken(PrepareUserClaims(user.Id, userBaseClaims), tokenExpirationDate);

        await UpdateUserToken(user, newToken, tokenExpirationDate);

        return Result<(string, DateTime)>.Ok((newToken, tokenExpirationDate));
    }

    private async Task UpdateUserToken(AuthenticationUser user, string newToken, DateTime tokenExpirationDate)
    {
        user.Token = newToken;
        user.TokenExpires = tokenExpirationDate;

        await _userManager.UpdateAsync(user);
    }

    private IList<Claim> PrepareUserClaims(string userId, IList<Claim> claims)
    {
        claims.Add(new Claim(JwtRegisteredClaimNames.NameId, userId));
        var validAudiences = _jwtSettings.ValidAudience.Split(';', StringSplitOptions.TrimEntries);
        foreach (var audience in validAudiences)
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
        return claims;
    }

    private string GenerateJwtToken(IEnumerable<Claim> userClaims, DateTime expirationDate)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.SecureCode);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(userClaims),
            Issuer = _jwtSettings.ValidIssuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Expires = expirationDate
        };
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static DateTime GenerateExpirationDate() => DateTime.UtcNow.AddYears(2);
    private static bool IsExpired(DateTime date) => DateTime.UtcNow >= date;
    private static bool IsLoginAndPasswordNotValid(string login, string password) => string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password);
}