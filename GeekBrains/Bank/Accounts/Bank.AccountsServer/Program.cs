using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;

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

RegisterDatabase(builder.Services);

RegisterMetrics(builder.Services, builder.Host);

var app = builder.Build();

if (!await TryInitializeDatabase(app.Services)) return -1;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
return 0;


static void RegisterDatabase(IServiceCollection services)
{
    services
        .AddDbContext<AccountsDbContext>((p, o) =>
        {
            var configuration = p.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionStringBuilder("EF").BuildWithDatabase();
            o.UseNpgsql(connectionString);
        })
        .AddScoped<EFAccountsStorageStrategy>();

    services
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

    services
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

    services.AddScoped<AccountsRepository>();
}

static async ValueTask<bool> TryInitializeDatabase(IServiceProvider provider)
{
    await using var scope = provider.CreateAsyncScope();
    try
    {
        await using var context = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
        await context.Database.MigrateAsync();

        var dapperContext = scope.ServiceProvider.GetRequiredService<PostgresDbInitializer>();
        await dapperContext.InitializeAccountsDatabase();

        var dapperMigration = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        dapperMigration.MigrateUp();
        return true;
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "Fail connect to databases");
        return false;

    }
}

static void RegisterMetrics(IServiceCollection services, IHostBuilder host)
{
    services
        .AddMetrics(metricsBuilder => metricsBuilder
            .OutputMetrics.AsPrometheusPlainText()
            .OutputMetrics.AsPrometheusProtobuf());

    host
        .UseMetricsWebTracking()
        .UseMetrics(o => o.EndpointOptions = mo =>
        {
            mo.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
            mo.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
            mo.EnvironmentInfoEndpointEnabled = false;
        });
}