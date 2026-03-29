using OrdSpel.Shared.GameDTOs;

namespace OrdSpel.BLL.Services
{
    public interface IGameService
    {
        Task<GameSessionResponseDto> CreateGameAsync(CreateGameDto dto, string userId);
        Task<GameSessionResponseDto?> JoinGameAsync(JoinGameDto dto, string userId);
        Task<GameSessionResponseDto?> GetGameAsync(string gameCode);
    }
}
