using MetricsManagement.Manager.Client;
using MetricsManagement.Manager.Data;
using MetricsManagement.Manager.Jobs;
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

app.Run();


static void ConfigureJobTrigger(ITriggerConfigurator t) => t
    .StartNow()
    .WithSimpleSchedule(s => s
        .RepeatForever()
        .WithIntervalInMinutes(4));