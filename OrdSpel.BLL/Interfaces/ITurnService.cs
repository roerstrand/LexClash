using OrdSpel.Shared.DTOs;

namespace OrdSpel.BLL.Interfaces
{
    public interface ITurnService
    {
        Task<(TurnResponseDto? response, string? error)> PlayTurnAsync(
            string gameCode, string userId, TurnRequestDto dto);
    }
}
