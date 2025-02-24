using Microsoft.EntityFrameworkCore;

namespace Thermocron.Data;

public class AppDbContext : DbContext
{
    private string _connectionString = "";

    public AppDbContext()
    {
    }

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Device> Devices { get; set; }
    public DbSet<Measure> Measures { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Measure>()
            .Property(e => e.Timestamp)
            .HasColumnType("timestamp without time zone");
    }
}