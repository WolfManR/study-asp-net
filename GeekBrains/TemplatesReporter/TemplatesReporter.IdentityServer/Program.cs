using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using TemplatesReporter.Authentication.Data;
using TemplatesReporter.AuthenticationRules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.ConfigureSwaggerAuthentication());

var configuration = builder.Configuration;
ConfigureDatabase(builder.Services, configuration);
builder.Services.RegisterCors();
builder.Services.ConfigureAuthentication(configuration);
builder.Services.AddScoped<AuthenticationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(JwtSettings.AuthPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    using var dbContext = scope.ServiceProvider.GetRequiredService<AuthorizationContext>();
    dbContext.Database.Migrate();
}

app.Run();

void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
{
    services.AddDbContext<AuthorizationContext>(options => options.UseNpgsql(configuration.GetConnectionString("Default")));

    services
        .AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 4;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<AuthorizationContext>()
        .AddDefaultTokenProviders();
}