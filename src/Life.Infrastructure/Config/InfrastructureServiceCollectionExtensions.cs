using Life.Domain.Repositories;
using Life.Infrastructure.Data;
using Life.Infrastructure.Repositories;
using Life.Infrastructure.Seeding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Life.Infrastructure.Config
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connStr)
        {
            return AddInfrastructure(services, connStr, useInMemoryDatabase: false);
        }
        
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connStr, bool useInMemoryDatabase)
        {
            if (useInMemoryDatabase)
            {
                services.AddDbContext<LifeDbContext>(o => o.UseInMemoryDatabase(connStr));
            }
            else
            {
                services.AddDbContext<LifeDbContext>(o => o.UseSqlServer(connStr));
            }
            
            // Infrastructure services only
            services.AddScoped<IBoardRepository, BoardRepository>();
            services.AddScoped<DatabaseSeeder>();
            
            return services;
        }
    }
}
