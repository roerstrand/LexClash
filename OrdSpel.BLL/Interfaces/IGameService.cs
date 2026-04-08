using OrdSpel.Shared;
using OrdSpel.Shared.DTOs;
using OrdSpel.Shared.GameDTOs;

namespace OrdSpel.BLL.Interfaces
{
    public interface IGameService
    {
        Task<GameSessionResponseDto> CreateGameAsync(CreateGameDto dto, string userId);
        Task<ServiceResult<GameSessionResponseDto>> JoinGameAsync(JoinGameDto dto, string userId);
        Task<ServiceResult<GameSessionResponseDto>> EndGameAsync(string gameCode, string userId);
        Task<GameSessionResponseDto?> GetGameAsync(string gameCode);
       Task<GameResultDto?> GetGameResultAsync(string gameCode);
    }
}
