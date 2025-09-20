using Life.Api.Extensions;
using Life.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.ConfigureApplication();

// Run database migrations and seeding (skip in test environment)
if (!app.Environment.IsEnvironment("Testing"))
{
    await app.SeedDatabaseAsync();
}

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
