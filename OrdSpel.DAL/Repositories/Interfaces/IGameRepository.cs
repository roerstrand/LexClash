using OrdSpel.Shared.GameDTOs;

namespace OrdSpel.DAL.Repositories.Interfaces
{
    public interface IGameRepository
    {
        Task<GameSessionResponseDto?> GetSessionByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code);
        Task<List<string>> GetWordsByCategoryAsync(int categoryId);
        Task<GameSessionResponseDto> CreateSessionAsync(string code, int categoryId, string startWord, string userId);
        Task AddPlayerAsync(string gameCode, string userId, int playerOrder);
        Task SetSessionActiveAsync(string gameCode);
        Task SetSessionFinishedAsync(string gameCode);
    }
}
