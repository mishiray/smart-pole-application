using DigitalTwinFramework.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Connect_Backend_API.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, string connectionString)
        {
            return
            services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
        }
    }
}
