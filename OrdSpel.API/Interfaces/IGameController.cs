using Microsoft.AspNetCore.Mvc;
using OrdSpel.Shared.DTOs;
using OrdSpel.Shared.GameDTOs;

namespace OrdSpel.API.Controllers
{
    public interface IGameController
    {
        Task<IActionResult> CreateGame([FromBody] CreateGameDto dto);
        Task<IActionResult> EndGame(string gameCode);
        Task<IActionResult> GetGame(string gameCode);
        Task<ActionResult<GameLobbyStatusDto>> GetLobbyStatus(string code);
        Task<IActionResult> JoinGame([FromBody] JoinGameDto dto);
    }
}