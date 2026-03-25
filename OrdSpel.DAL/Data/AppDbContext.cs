using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OrdSpel.DAL.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
          : base(options)
        {
        }

        public DbSet<Category> Categories{ get; set; }
        public DbSet<GamePlayer> GamePlayers { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<GameTurn> GameTurns{ get; set; }
        public DbSet<Word> Words { get; set; } 





    }
}
