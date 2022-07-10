using Catalog.Site;
using Catalog.Site.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddRefitClient<IBooksCatalogClient>().ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5267"));

await builder.Build().RunAsync();
