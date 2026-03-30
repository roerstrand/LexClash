using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdSpel.BLL.Services;
using OrdSpel.Shared.GameDTOs;
using System.Security.Claims;

namespace OrdSpel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _gameService.CreateGameAsync(dto, userId);
            return Ok(result);
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinGame([FromBody] JoinGameDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _gameService.JoinGameAsync(dto, userId);
            if (result == null)
                return BadRequest("Spelet hittades inte, är fullt eller har redan startat.");

            return Ok(result);
        }

        [HttpGet("{gameCode}")]
        public async Task<IActionResult> GetGame(string gameCode)
        {
            var result = await _gameService.GetGameAsync(gameCode);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
