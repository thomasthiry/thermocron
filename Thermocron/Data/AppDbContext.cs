using Microsoft.EntityFrameworkCore;

namespace Thermocron.Data;

public class AppDbContext : DbContext
{
    public DbSet<Device> Devices { get; set; }
    public DbSet<Measure> Measures { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost:15432;Database=thermocron_dev;Username=thermocron_dev;Password=thermocron");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Measure>()
            .Property(e => e.Timestamp)
            .HasColumnType("timestamp without time zone");
    }
}