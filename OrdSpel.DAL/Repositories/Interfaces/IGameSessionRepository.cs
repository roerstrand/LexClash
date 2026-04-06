using OrdSpel.DAL.Models;

namespace OrdSpel.DAL.Repositories.Interfaces
{
    public interface IGameSessionRepository
    {
        Task<GameSession?> GetByGameCodeAsync(string gameCode);
        Task<GameSession?> GetByGameCodeWithLobbyAsync(string gameCode);
        Task<GameSession?> GetByGameCodeWithDetailsAsync(string gameCode);
    }
}
