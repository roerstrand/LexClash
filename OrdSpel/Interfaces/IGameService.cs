using OrdSpel.Shared.GameDTOs;

namespace OrdSpel.UI.Interfaces
{
    public interface IGameService
    {
        Task<GameSessionResponseDto?> CreateGameAsync(CreateGameDto dto);
        Task<List<T>> GetCategoriesAsync<T>();
        Task<GameSessionResponseDto?> GetGameAsync(string gameCode);
        Task<(GameSessionResponseDto? Result, string? Error)> JoinGameAsync(JoinGameDto dto);
    }
}