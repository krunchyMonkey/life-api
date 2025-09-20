using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Life.Infrastructure.Data;

namespace Life.Api.Tests.Infrastructure;

/// <summary>
/// Custom WebApplicationFactory for integration testing with in-memory database
/// </summary>
public class LifeApiTestFactory : WebApplicationFactory<Program>
{
    private string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Use unique database name for each test
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _databaseName,
                ["Final:MaxIterations"] = "1000",
                ["Final:MaxMillis"] = "5000"
            });
        });
    }

    /// <summary>
    /// Create a new test instance with a fresh database
    /// </summary>
    public static LifeApiTestFactory Create()
    {
        return new LifeApiTestFactory
        {
            _databaseName = Guid.NewGuid().ToString()
        };
    }

    /// <summary>
    /// Get the database context for direct database operations in tests
    /// </summary>
    public LifeDbContext GetDbContext()
    {
        // Get a scoped DbContext that shares the same database as the test client
        var serviceProvider = Services.CreateScope().ServiceProvider;
        var context = serviceProvider.GetRequiredService<LifeDbContext>();
        
        // Ensure the database is created
        context.Database.EnsureCreated();
        
        return context;
    }

    /// <summary>
    /// Clean up the database
    /// </summary>
    public async Task CleanupDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LifeDbContext>();
        
        // Remove all data
        context.Snapshots.RemoveRange(context.Snapshots);
        context.Boards.RemoveRange(context.Boards);
        await context.SaveChangesAsync();
    }
}