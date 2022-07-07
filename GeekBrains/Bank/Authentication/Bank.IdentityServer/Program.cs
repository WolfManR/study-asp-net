using Bank.Authentication.Data;
using Bank.AuthenticationRules;
using Bank.IdentityServer.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

const string corsPolicyAlias = "AuthPolicy";
const string refreshTokenCookieId = "refreshToken";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.ConfigureSwaggerAuthentication());

var configuration = builder.Configuration;
builder.Services
    .AddDbContext<AuthenticationDbContext>((provider, options) =>
    {
        var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("Identity");
        options.UseNpgsql(connectionString);
    })
    .AddIdentity<AuthenticationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 4;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<AuthenticationDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddScoped<AuthenticationService>()
    .AddScoped<AuthorizationHelper>();

builder.Services.RegisterBaseCors(corsPolicyAlias);
builder.Services.ConfigureAuthentication(configuration);

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("auth", policyBuilder => policyBuilder.
        RequireAuthenticatedUser()
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyAlias);
app.UseAuthentication();
app.UseAuthorization();

await using (var scope = app.Services.CreateAsyncScope())
{
    await using var context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
    await context.Database.MigrateAsync();
}

app.MapPost("signin", async static ([FromBody] Credentials credentials, HttpContext context, AuthenticationService authenticationService) =>
{
    var result = await authenticationService.Authenticate(credentials.Login, credentials.Password);
    if (!result.IsSuccess) return Results.BadRequest();

    TokenData tokenData = result.GetResult();

    CookieOptions refreshTokenCookieOptions = new()
    {
        HttpOnly = true,
        Expires = tokenData.RefreshTokenExpirationDate
    };
    context.Response.Cookies.Append(refreshTokenCookieId, tokenData.RefreshToken, refreshTokenCookieOptions);

    return Results.Ok(tokenData.Token);
});

app.MapPost("signup", async static ([FromBody] Credentials credentials, AuthenticationService authenticationService) =>
{
    var isSucceed = await authenticationService.RegisterUser(credentials.Login, credentials.Password);
    if (!isSucceed) return Results.BadRequest();

    return Results.Ok();
});

app.MapPost("refresh-token", async static (HttpContext context, AuthenticationService authenticationService) =>
{
    if (!context.Request.Cookies.TryGetValue(refreshTokenCookieId, out var oldRefreshToken)) return Results.Unauthorized();
    var result = await authenticationService.RefreshToken(oldRefreshToken!);
    if (!result.IsSuccess) return Results.Unauthorized();

    (string newRefreshToken, DateTime newExpiration) = result.GetResult();

    CookieOptions refreshTokenCookieOptions = new()
    {
        HttpOnly = true,
        Expires = newExpiration
    };
    context.Response.Cookies.Append(refreshTokenCookieId, newRefreshToken, refreshTokenCookieOptions);
    return Results.Ok();
}).RequireAuthorization("auth");

app.Run();