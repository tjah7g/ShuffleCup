using GameApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PlayerScore> PlayerScores => Set<PlayerScore>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerScore>(entity =>
            {
                entity.Property(p => p.PlayerName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(p => p.Score).IsRequired();
                entity.Property(p => p.Level).HasDefaultValue(1); // TAMBAHAN: Default level 1
                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Index untuk query leaderboard (Score desc, Level desc, Id asc)
                entity.HasIndex(p => new { p.Score, p.Level, p.Id });
            });

            // Seed data dengan Level
            modelBuilder.Entity<PlayerScore>().HasData(
  new PlayerScore { Id = 1, PlayerName = "Andi", Score = 1200, Level = 5, CreatedAt = new DateTime(2024, 1, 1) },
  new PlayerScore { Id = 2, PlayerName = "Nadia", Score = 980, Level = 4, CreatedAt = new DateTime(2024, 1, 2) }
);
        }
    }
}
