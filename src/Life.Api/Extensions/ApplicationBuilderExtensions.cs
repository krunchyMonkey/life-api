using Life.Api.Filters;
using Life.Infrastructure.Seeding;

namespace Life.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures exception handling middleware
    /// </summary>
    public static WebApplication ConfigureExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<ApiExceptionMiddleware>();
        
        return app;
    }

    /// <summary>
    /// Configures the development environment pipeline
    /// </summary>
    public static WebApplication ConfigureDevelopmentPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        return app;
    }

    /// <summary>
    /// Configures standard middleware pipeline
    /// </summary>
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        
        return app;
    }

    /// <summary>
    /// Configures API endpoints
    /// </summary>
    public static WebApplication ConfigureEndpoints(this WebApplication app)
    {
        app.MapControllers();
        
        return app;
    }

    /// <summary>
    /// Seeds the database with initial data
    /// </summary>
    public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
        
        return app;
    }

    /// <summary>
    /// Configures the complete application pipeline
    /// </summary>
    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        app.ConfigureExceptionHandling()
           .ConfigureDevelopmentPipeline()
           .ConfigureMiddleware()
           .ConfigureEndpoints();
           
        return app;
    }
}