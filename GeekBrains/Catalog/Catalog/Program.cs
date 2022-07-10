using Catalog.Books;
using Catalog.Books.Data;
using Catalog.FullTextSearch;
using Neo4j.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<Neo4jOptions>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();

        return new Neo4jOptions()
        {
            Uri = configuration.GetValue<string>("ConnectionStrings:Neo4j:Uri"),
            Login = configuration.GetValue<string>("ConnectionStrings:Neo4j:Login"),
            Password = configuration.GetValue<string>("ConnectionStrings:Neo4j:Password"),
        };
    })
    .AddScoped<Neo4jContext>()
    .AddScoped<IBooksRepository, BooksRepository>();

var elasticConfiguration = builder.Configuration.GetSection("Elastic");
builder.Services.Configure<ElasticConfiguration>(elasticConfiguration);
builder.Services.AddScoped<ElasticService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
