using MetricsManagement.Manager.Client;
using MetricsManagement.Manager.Data;
using MetricsManagement.Manager.Jobs;

using Microsoft.AspNetCore.Mvc;

using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<MetricsClient>();
builder.Services.AddTransient<AgentsRepository>();
builder.Services.AddTransient<MetricsRepository>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.ScheduleJob<ProcessorTimeJob>(ConfigureJobTrigger);
});

builder.Services.AddQuartzHostedService(options =>
{
    // when shutting down we want jobs to complete gracefully
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("agent/register", ([FromBody] string uri, [FromServices] AgentsRepository agentsRepository) =>
{
    var id = agentsRepository.Register(uri);
    return Results.Ok(id);
});

app.MapPost("agent/{agentId}/enable", (int agentId, [FromServices] AgentsRepository agentsRepository) =>
{
    agentsRepository.Enable(agentId);
    return Results.Ok();
});

app.MapPost("agent/{agentId}/disable", (int agentId, [FromServices] AgentsRepository agentsRepository) =>
{
    agentsRepository.Enable(agentId);
    return Results.Ok();
});

app.MapPost("agents", ([FromServices] AgentsRepository agentsRepository) =>
{
    var agents = agentsRepository.Get();
    return Results.Ok(agents);
});

app.MapGet("metrics", (int agentId, [FromServices] MetricsRepository repository) =>
{
    var metrics = repository.Get(agentId, DateTimeOffset.Now.AddHours(-12), DateTimeOffset.Now);
    return Results.Ok(metrics);
});

app.Run();


static void ConfigureJobTrigger(ITriggerConfigurator t) => t
    .StartNow()
    .WithSimpleSchedule(s => s
        .RepeatForever()
        .WithIntervalInMinutes(1));