using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Models;
using OrdSpel.DAL.Repositories.Interfaces;

namespace OrdSpel.DAL.Repositories
{
    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly AppDbContext _context;

        public GameSessionRepository(AppDbContext context)
        {
            _context = context;
        }

        // Lightter without Include in case of only need GameCode
        public Task<GameSession?> GetByGameCodeAsync(string gameCode)
        {
            if (string.IsNullOrWhiteSpace(gameCode))
            {
                return Task.FromResult<GameSession?>(null);
            }

            return _context.GameSessions.FirstOrDefaultAsync(s => s.GameCode == gameCode);
        }

        public Task<GameSession?> GetByGameCodeWithLobbyAsync(string gameCode)
        {
            if (string.IsNullOrWhiteSpace(gameCode))
            {
                return Task.FromResult<GameSession?>(null);
            }

            // Retrieve GameCode along with related Category and Players for lobby display
            return _context.GameSessions
                .Include(s => s.Category)
                .Include(s => s.Players)
                .FirstOrDefaultAsync(s => s.GameCode == gameCode);
        }
        public Task<GameSession?> GetByGameCodeWithDetailsAsync(string gameCode)
        {
            if (string.IsNullOrWhiteSpace(gameCode))
            {
                return Task.FromResult<GameSession?>(null);
            }

            return _context.GameSessions
                .Include(s => s.Players)
                .Include(s => s.Turns)
                .FirstOrDefaultAsync(s => s.GameCode == gameCode);
        }
    }
}
