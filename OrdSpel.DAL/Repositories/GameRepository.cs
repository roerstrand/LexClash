using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Models;
using OrdSpel.Shared;
using OrdSpel.Shared.GameDTOs;

namespace OrdSpel.DAL.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly AppDbContext _db;

        public GameRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<GameSessionResponseDto?> GetSessionByCodeAsync(string code)
        {
            var session = await _db.GameSessions
                .Include(s => s.Players)
                .FirstOrDefaultAsync(s => s.GameCode == code);

            if (session == null) return null;

            return MapToDto(session);
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _db.GameSessions.AnyAsync(s => s.GameCode == code);
        }

        public async Task<List<string>> GetWordsByCategoryAsync(int categoryId)
        {
            return await _db.Words
                .Where(w => w.CategoryId == categoryId)
                .Select(w => w.Text)
                .ToListAsync();
        }

        public async Task<GameSessionResponseDto> CreateSessionAsync(string code, int categoryId, string startWord, string userId)
        {
            var session = new GameSession
            {
                GameCode = code,
                CategoryId = categoryId,
                StartWord = startWord,
                Status = GameStatus.Waiting,
                CurrentRound = 0,
                CreatedAt = DateTime.UtcNow
            };

            _db.GameSessions.Add(session);
            await _db.SaveChangesAsync();

            var player = new GamePlayer
            {
                SessionId = session.Id,
                UserId = userId,
                PlayerOrder = 1
            };

            _db.GamePlayers.Add(player);
            await _db.SaveChangesAsync();

            session.Players.Add(player);
            return MapToDto(session);
        }

        public async Task AddPlayerAsync(string gameCode, string userId, int playerOrder)
        {
            var session = await _db.GameSessions
                .FirstOrDefaultAsync(s => s.GameCode == gameCode);

            if (session == null) return;

            var player = new GamePlayer
            {
                SessionId = session.Id,
                UserId = userId,
                PlayerOrder = playerOrder
            };

            _db.GamePlayers.Add(player);
            await _db.SaveChangesAsync();
        }

        public async Task SetSessionActiveAsync(string gameCode)
        {
            var session = await _db.GameSessions
                .FirstOrDefaultAsync(s => s.GameCode == gameCode);

            if (session == null) return;

            session.Status = GameStatus.Active;
            await _db.SaveChangesAsync();
        }

        private static GameSessionResponseDto MapToDto(GameSession session)
        {
            return new GameSessionResponseDto
            {
                GameCode = session.GameCode,
                Status = session.Status,
                CategoryId = session.CategoryId,
                StartWord = session.StartWord,
                PlayerIds = session.Players.Select(p => p.UserId).ToList()
            };
        }
    }
}
