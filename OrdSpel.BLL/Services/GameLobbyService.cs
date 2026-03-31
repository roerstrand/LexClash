using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Data;
using OrdSpel.Shared;
using OrdSpel.Shared.DTOs;
using System.Threading.Tasks;

namespace OrdSpel.BLL.Services
{
    // This Service is for retrieving the game lobby status from the database
    // and mapping it to a GameLobbyStatusDto to be returned for API/UI consumption

    public class GameLobbyService : IGameLobbyService
    {
        private readonly AppDbContext _context;

        public GameLobbyService(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves the game lobby status for a given game code
        public async Task<GameLobbyStatusDto?> GetLobbyStatusAsync(string gameCode)
        {
            if (string.IsNullOrWhiteSpace(gameCode))
            {
                return null;
            }
            // Query the database for the game session with the specified game code,
            // including related category and players
            var session = await _context.GameSessions
                .Include(s => s.Category)
                .Include(s => s.Players)
                .FirstOrDefaultAsync(s => s.GameCode == gameCode);

            if (session == null)
            {
                return null;
            }

            // Map the retrieved game session to a GameLobbyStatusDto
            return new GameLobbyStatusDto
            {
                SessionId = session.Id,
                GameCode = session.GameCode,
                CategoryName = session.Category?.Name ?? string.Empty,
                StartWord = session.StartWord,
                Status = session.Status,
                PlayerCount = session.Players.Count,
                MaxPlayers = 2,
                IsReadyToStart = session.Players.Count >= 2 && session.Status == GameStatus.InProgress,
                CurrentTurnUserId = session.CurrentTurnUserId,
              
            };
        }
    }
}