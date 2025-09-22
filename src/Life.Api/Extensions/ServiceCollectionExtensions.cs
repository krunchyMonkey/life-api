using FluentValidation;
using Life.Application.Board.Pipeline;
using Life.Infrastructure.Config;
using MediatR;
using Microsoft.OpenApi.Models;

namespace Life.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds API services including controllers, API explorer, and problem details
    /// </summary>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();
        
        return services;
    }

    /// <summary>
    /// Adds OpenAPI/Swagger documentation services
    /// </summary>
    public static IServiceCollection AddDocumentationServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "Life API", 
                Version = "v1",
                Description = "Conway's Game of Life API"
            });
        });
        
        return services;
    }

    /// <summary>
    /// Adds CORS services with permissive policy for development
    /// </summary>
    public static IServiceCollection AddCorsServices(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
        
        return services;
    }

    /// <summary>
    /// Adds MediatR with validation pipeline behavior
    /// </summary>
    public static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(Life.Application.Mapper).Assembly));

        services.AddValidatorsFromAssembly(typeof(Life.Application.Mapper).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        return services;
    }

    /// <summary>
    /// Adds infrastructure services with configuration
    /// Domain services are now registered in the Infrastructure layer
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=LifeDb;Trusted_Connection=true;MultipleActiveResultSets=true";

        // Use InMemory database for Testing environment
        var environment = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        bool useInMemory = environment == "Testing";

        services.AddInfrastructure(connectionString, useInMemory);
        
        return services;
    }

    /// <summary>
    /// Adds all application services in the correct order
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiServices()
                .AddDocumentationServices()
                .AddCorsServices()
                .AddMediatRServices()
                .AddInfrastructureServices(configuration);
                
        return services;
    }
}