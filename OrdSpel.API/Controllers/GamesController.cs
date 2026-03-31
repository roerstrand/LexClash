using Microsoft.AspNetCore.Mvc;
using OrdSpel.BLL.Services;
using OrdSpel.Shared.DTOs;

namespace OrdSpel.API.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ControllerBase
    {
        private readonly IGameLobbyService _gameLobbyService;

        public GamesController(IGameLobbyService gameLobbyService)
        {
            _gameLobbyService = gameLobbyService;
        }

        // Endpoint to get the status of a game lobby by its code(gameCode for GameSession): Code will be binded from the URL path, e.g., /api/games/ABCD/lobby
        // GET /api/games/ABC123/lobby
        [HttpGet("{code}/lobby")]
        public async Task<ActionResult<GameLobbyStatusDto>> GetLobbyStatus(string code)
        {
            var result = await _gameLobbyService.GetLobbyStatusAsync(code);

            if (result == null)
            {
                return NotFound("Game session was not found.");
            }

            return Ok(result);
        }
    }
}
