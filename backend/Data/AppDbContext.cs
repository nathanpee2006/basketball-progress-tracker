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
        public DbSet<Achievement> Achievements { get; set; } = null!;
        public DbSet<PlayerAchievement> PlayerAchievements { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add constraints and indexes
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.ClerkUserId)
                .IsUnique();

            modelBuilder.Entity<Player>()
                .HasIndex(p => p.Email)
                .IsUnique();

            modelBuilder.Entity<Session>()
                .HasIndex(s => new { s.PlayerId, s.Date });

            modelBuilder.Entity<PlayerAchievement>()
                .HasIndex(pa => new { pa.PlayerId, pa.AchievementId })
                .IsUnique();

            modelBuilder.Entity<PlayerAchievement>()
                .HasOne<Player>()
                .WithMany()
                .HasForeignKey(pa => pa.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerAchievement>()
                .HasOne<Achievement>()
                .WithMany()
                .HasForeignKey(pa => pa.AchievementId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
