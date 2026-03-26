using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Models;

namespace OrdSpel.DAL.Data
{
    public class AppDbContext : DbContext
    {

       
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }
     

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<GamePlayer> GamePlayers { get; set; } = null!;
        public DbSet<GameSession> GameSessions { get; set; } = null!;
        public DbSet<GameTurn> GameTurns { get; set; } = null!;
        public DbSet<Word> Words { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            builder.Entity<Word>(entity =>
            {
                entity.Property(w => w.Text)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(w => new { w.CategoryId, w.Text })
                    .IsUnique();

                entity.HasOne<Category>()
                    .WithMany(c => c.Words)
                    .HasForeignKey(w => w.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<GameSession>(entity =>
            {
                entity.Property(g => g.GameCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasIndex(g => g.GameCode)
                    .IsUnique();

                entity.Property(g => g.StartWord)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(g => g.Category)
                    .WithMany(c => c.GameSessions)
                    .HasForeignKey(g => g.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<GamePlayer>(entity =>
            {
                entity.HasOne(gp => gp.Session)
                    .WithMany(gs => gs.Players)
                    .HasForeignKey(gp => gp.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(gp => new { gp.SessionId, gp.UserId })
                    .IsUnique();

                entity.HasIndex(gp => new { gp.SessionId, gp.PlayerOrder })
                    .IsUnique();
            });

            builder.Entity<GameTurn>(entity =>
            {
                entity.Property(gt => gt.Word)
                    .HasMaxLength(100);

                entity.HasOne(gt => gt.Session)
                    .WithMany(gs => gs.Turns)
                    .HasForeignKey(gt => gt.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}
