using Microsoft.AspNetCore.Components.Authorization;

using RazorEngineCore;

using TemplatesReporter.ApiClients;
using TemplatesReporter.Mail.Core;
using TemplatesReporter.Mail.MailKit;
using TemplatesReporter.Mail.RazorEngineCore;
using TemplatesReporter.Site.Data;
using TemplatesReporter.Site.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var urls = builder.Configuration.GetSection(nameof(Urls)).Get<Urls>();

// register authentication state provider in that manner to simplify it access
builder.Services.AddScoped<WebApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<WebApiAuthenticationStateProvider>());

builder.Services.AddHttpClient<AuthenticationService>(client => client.BaseAddress = new Uri(urls.Identity));
builder.Services.AddHttpClient<EmailSendService>(client => client.BaseAddress = new Uri(urls.MailScheduler));

builder.Services.AddSingleton<IRazorEngine, RazorEngine>();
builder.Services.AddSingleton<ITemplateBuilder, MailContentBuilder>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<ModelsGenerator>();
builder.Services.AddSingleton<TemplatesRepository>();

builder.Services.AddAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
