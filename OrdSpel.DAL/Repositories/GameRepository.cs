using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Models;
using OrdSpel.DAL.Repositories.Interfaces;
using OrdSpel.Shared.Enums;
using OrdSpel.Shared.DTOs;
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

        public async Task<GameResultDto?> GetGameResultAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            var session = await _db.GameSessions
                .Include(s => s.Players)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.GameCode == code);

            if (session is null)
            {
                return null;
            }

            if (session.Status != GameStatus.GameFinished)
            {
                return new GameResultDto
                {
                    GameCode = session.GameCode,
                    Status = session.Status,
                    Players = [],
                    WinnerUserId = null
                };
            }

            var players = session.Players
                .OrderBy(p => p.PlayerOrder)
                .Select(p => new GamePlayerStatusDto(p.UserId, p.PlayerOrder, p.TotalScore))
                .ToList();

            string? winnerUserId = null;
            if (players.Count > 0)
            {
                var maxScore = players.Max(p => p.TotalScore);
                var topPlayers = players.Where(p => p.TotalScore == maxScore).ToList();
                winnerUserId = topPlayers.Count == 1 ? topPlayers[0].UserId : null;
            }

            return new GameResultDto
            {
                GameCode = session.GameCode,
                Status = session.Status,
                Players = players,
                WinnerUserId = winnerUserId
            };
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
                Status = GameStatus.WaitingForPlayers,
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
                .Include(s => s.Players)
                .FirstOrDefaultAsync(s => s.GameCode == gameCode);

            if (session == null) return;

            session.Status = GameStatus.InProgress;
            if (session.CurrentRound <= 0)
            {
                session.CurrentRound = 1;
            }

            var firstPlayerId = session.Players
                .OrderBy(p => p.PlayerOrder)
                .Select(p => p.UserId)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(firstPlayerId))
            {
                session.CurrentTurnUserId = firstPlayerId;
            }
            await _db.SaveChangesAsync();
        }

        public async Task SetSessionFinishedAsync(string gameCode)
        {
            var session = await _db.GameSessions
                .FirstOrDefaultAsync(s => s.GameCode == gameCode);

            if (session == null) return;

            session.Status = GameStatus.GameFinished;
            await _db.SaveChangesAsync();
        }

        public async Task<GameSessionResponseDto?> GetActiveSessionByUserAsync(string userId)
        {
            var session = await _db.GameSessions
                .Include(s => s.Players)
                .Where(s => s.Players.Any(p => p.UserId == userId))
                .Where(s => s.Status == GameStatus.WaitingForPlayers || s.Status == GameStatus.InProgress)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (session == null) return null;

            return MapToDto(session);
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
