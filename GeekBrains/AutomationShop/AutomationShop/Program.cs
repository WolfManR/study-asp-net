using AutomationShop.Models;

using JsonMemoryCache;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<JsonMemoryCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var cacheService = scope.ServiceProvider.GetRequiredService<JsonMemoryCacheService>();
    SeedData(cacheService);
}

app.Run();

static void SeedData(JsonMemoryCacheService cacheService)
{
    cacheService.Create(new ProductViewModel
    {
        Title = "Make Your business To Be Better Famous In Internet",
        Icon = "images/Features1.svg",
        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
    });

    cacheService.Create(new ProductViewModel
    {
        Title = "Bring Technology To Your Comfortable Home",
        Icon = "images/Features2.svg",
        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
    });

    cacheService.Create(new ProductViewModel
    {
        Title = "Build Your Digital Product That Suitable For Your Need",
        Icon = "images/Features3.svg",
        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
    });

    cacheService.Create(new FinishedProductViewModel()
    {
        Title = "Smart Home Installation",
        Icon = "images/SmartHomeBackground.png",
        IsIdol = true
    });

    cacheService.Create(new FinishedProductViewModel()
    {
        Title = "Sparklite App",
        Icon = "images/SparkliteAppBackground.png"
    });

    cacheService.Create(new FinishedProductViewModel()
    {
        Title = "Car-Rapetition App",
        Icon = "images/CarRapetitionAppBackground.png"
    });
}