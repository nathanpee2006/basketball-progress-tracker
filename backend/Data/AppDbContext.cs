using Backend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<Drill> Drills { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add constraints and indexes
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.ClerkUserId)
                .IsUnique();

            modelBuilder.Entity<Session>()
                .HasIndex(s => new { s.PlayerId, s.Date });
        }
    }
}
