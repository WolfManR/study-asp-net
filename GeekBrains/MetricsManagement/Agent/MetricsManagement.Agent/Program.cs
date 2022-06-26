using MetricsManagement.Agent.Data;
using MetricsManagement.Agent.Jobs;

using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<Repository>();

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

app.MapGet("windows/processor-time/total", (Repository repository) =>
{
    var metrics = repository.Get(DateTimeOffset.Now.AddHours(-12), DateTimeOffset.Now);
    return Results.Ok(metrics);
});

app.Run();

static void ConfigureJobTrigger(ITriggerConfigurator t) => t
        .StartNow()
        .WithSimpleSchedule(s => s.RepeatForever()
        .WithIntervalInMinutes(1));