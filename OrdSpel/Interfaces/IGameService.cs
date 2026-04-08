using OrdSpel.Shared.DTOs;
using OrdSpel.Shared.GameDTOs;
using OrdSpel.Shared.DTOs;

namespace OrdSpel.UI.Interfaces
{
    public interface IGameService
    {
        Task<GameSessionResponseDto?> CreateGameAsync(CreateGameDto dto);
        Task<List<T>> GetCategoriesAsync<T>();
        Task<GameSessionResponseDto?> GetGameAsync(string gameCode);
        Task<(GameSessionResponseDto? Result, string? Error)> JoinGameAsync(JoinGameDto dto);
        Task<GameStatusDto?> GetGameStatusAsync(string gameCode);
        Task<GameResultDto?> GetGameResultAsync(string gameCode);
        Task<GameStatusDto?> GetGameStatusAsync(string gameCode);
        Task<(TurnResponseDto? Result, string? Error)> SubmitTurnAsync(string gameCode, TurnRequestDto dto);
    }
}