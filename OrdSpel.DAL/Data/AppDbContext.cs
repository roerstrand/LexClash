using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Models;

namespace OrdSpel.DAL.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {

        // Inherite from IdentityDbContext to include ASP.NET Core Identity tables for user management
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }
     

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<GamePlayer> GamePlayers { get; set; } = null!;
        public DbSet<GameSession> GameSessions { get; set; } = null!;
        public DbSet<GameTurn> GameTurns { get; set; } = null!;
        public DbSet<Word> Words { get; set; } = null!;
    }
}
