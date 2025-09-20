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
            services.AddDbContext<LifeDbContext>(o => o.UseSqlServer(connStr));
            services.AddScoped<IBoardRepository, BoardRepository>();
            services.AddScoped<DatabaseSeeder>();
            // services.AddScoped<IBoardStoreRepository, BoardStoreRepository>(); // TODO: Implement when needed
            return services;
        }
    }
}
