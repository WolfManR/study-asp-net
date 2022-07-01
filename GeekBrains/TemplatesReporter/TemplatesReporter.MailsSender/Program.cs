using Quartz;
using TemplatesReporter.AuthenticationRules;
using TemplatesReporter.Mail.Core;
using TemplatesReporter.Mail.MailKit;
using TemplatesReporter.MailsSender.Data;
using TemplatesReporter.MailsSender.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.ConfigureSwaggerAuthentication());

var domainConfiguration = builder.Configuration.GetSection("Domain");
builder.Services.Configure<EmailConfiguration>(domainConfiguration);
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<EmailsRepository>();
builder.Services.AddSingleton<AuthorizationHelper>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.ScheduleJob<MailSendJob>(trigger => trigger
        .StartNow()
        .WithSimpleSchedule(builder => builder.RepeatForever().WithIntervalInMinutes(3))
    );
});

builder.Services.AddQuartzHostedService(options =>
{
    // when shutting down we want jobs to complete gracefully
    options.WaitForJobsToComplete = true;
});

builder.Services.RegisterCors();

builder.Services.ConfigureAuthentication(builder.Configuration);

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

app.Run();
