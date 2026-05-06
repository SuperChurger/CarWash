using Microsoft.EntityFrameworkCore;

namespace CarWash;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<CarWash> CarWashes { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasIndex(b => new { b.CarWashId, b.StartTime })
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.CarWash)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CarWashId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}