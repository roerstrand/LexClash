using OrdSpel.BLL.Interfaces;
using OrdSpel.DAL.Repositories.Interfaces;
using OrdSpel.Shared.DTOs;
using OrdSpel.Shared.Enums;
using System.Threading.Tasks;

namespace OrdSpel.BLL.Services
{
    // This Service is for retrieving the game lobby status from the database
    // and mapping it to a GameLobbyStatusDto to be returned for API/UI consumption

    public class GameLobbyService : IGameLobbyService
    {
        private readonly IGameSessionRepository _gameSessionRepository;

        public GameLobbyService(IGameSessionRepository gameSessionRepository)
        {
            _gameSessionRepository = gameSessionRepository;
        }

        // Retrieves the game lobby status for a given game code
        public async Task<GameLobbyStatusDto?> GetLobbyStatusAsync(string gameCode)
        {
            if (string.IsNullOrWhiteSpace(gameCode))
            {
                return null;
            }

            // Retrieve the game session including related category and players
            var session = await _gameSessionRepository.GetByGameCodeWithLobbyAsync(gameCode);

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