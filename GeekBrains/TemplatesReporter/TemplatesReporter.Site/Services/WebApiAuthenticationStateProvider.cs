using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace TemplatesReporter.Site.Services;

internal class WebApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorageService;

    public WebApiAuthenticationStateProvider(ProtectedLocalStorage localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var result = await _localStorageService.GetAsync<string>("accessToken");

        var identity = result.Success && result.Value is { } accessToken
            ? GetClaimsIdentity(accessToken)
            : new ClaimsIdentity();

        var claimsPrincipal = new ClaimsPrincipal(identity);

        return await Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await _localStorageService.SetAsync("accessToken", token);

        var identity = GetClaimsIdentity(token);

        var claimsPrincipal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorageService.DeleteAsync("accessToken");

        var identity = new ClaimsIdentity();

        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    private ClaimsIdentity GetClaimsIdentity(string token)
    {
        var tokenData = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var claims = tokenData.Claims;

        var claimsIdentity = new ClaimsIdentity(claims, "apiauth_type");

        return claimsIdentity;
    }
}