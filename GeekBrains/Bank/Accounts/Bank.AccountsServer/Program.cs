using Bank.Accounts.Data;
using Bank.Accounts.Data.Dapper;
using Bank.Accounts.Data.EF;
using Bank.AccountsServer.Database;

using FluentMigrator.Runner;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddDbContext<AccountsDbContext>((p, o) =>
    {
        var configuration = p.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionStringBuilder("EF").BuildWithDatabase();
        o.UseNpgsql(connectionString);
    })
    .AddScoped<EFAccountsStorageStrategy>();

builder.Services
    .AddScoped<IDbConnector, PostgresConnector>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionStringBuilder("Dapper").BuildWithDatabase();
        return new(connectionString);
    })
    .AddScoped<PostgresDbInitializer>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionStringBuilder("Dapper");
        return new(connectionString);
    })
    .AddScoped<DapperAccountsStorageStrategy>();

builder.Services
    .AddFluentMigratorCore()
    .ConfigureRunner(c => c
        .AddPostgres()
        .WithGlobalConnectionString(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionStringBuilder("Dapper").BuildWithDatabase();
            return connectionString;
        })
        .ScanIn(typeof(Program).Assembly).For.Migrations());

builder.Services.AddScoped<AccountsRepository>();


var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    try
    {
        await using var context = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
        await context.Database.MigrateAsync();

        var dapperContext = scope.ServiceProvider.GetRequiredService<PostgresDbInitializer>();
        await dapperContext.InitializeAccountsDatabase();

        var dapperMigration = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        dapperMigration.MigrateUp();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "Fail connect to databases");
        Environment.Exit(-1);
        return;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
