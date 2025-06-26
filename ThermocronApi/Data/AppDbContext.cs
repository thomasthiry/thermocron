using Microsoft.EntityFrameworkCore;

namespace ThermocronApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Device> Devices { get; set; }
    public DbSet<Measure> Measures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Measure>()
            .Property(e => e.Timestamp)
            .HasColumnType("timestamp without time zone");
    }
}
