using Microsoft.AspNetCore.Mvc;

using TemplatesReporter.Authentication.Data;

namespace TemplatesReporter.IdentityServer.Controllers;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;

    public AuthController(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("signin")]
    public async Task<IActionResult> Authenticate([FromBody] Credentials credentials)
    {
        string token = await _authenticationService.Authenticate(credentials.Login, credentials.Password);
        if (token is null)
        {
            return BadRequest(new { message = "Username or password is incorrect" });
        }
        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] Credentials credentials)
    {
        var succeed = await _authenticationService.RegisterUser(credentials.Login, credentials.Password);
        return succeed ? Ok() : BadRequest();
    }
}

public record Credentials(string Login, string Password);