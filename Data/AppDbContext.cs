using DigitalTwinFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalTwinFramework.Data
{
    public class AppDbContext : DbContext
    {

        public DbSet<DHT11Sensor> DHT11Sensors { get; set; }

        public DbSet<UltrasonicSensor> UltrasonicSensors { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
