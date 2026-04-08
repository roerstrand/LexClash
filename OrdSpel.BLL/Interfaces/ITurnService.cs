using OrdSpel.Shared.DTOs;

namespace OrdSpel.BLL.Interfaces
{
    public interface ITurnService
    {
        //async metod som måste implementeras i TurnService
        //metoden returnerar två värden (en tuple), ett response av typen TurnResponseDto och ett error men man får alltid bara ett tillbaka; antingen en dto med speldata eller ett felmeddelande (via if sats i controller)
        //tar in 3 parametrar, en gamecode, userid och en dto
        Task<(TurnResponseDto? response, string? error)> PlayTurnAsync(
            string gameCode, string userId, TurnRequestDto dto);
    }
}
