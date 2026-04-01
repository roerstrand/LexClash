using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdSpel.API.Interfaces;
using OrdSpel.BLL.Interfaces;
using OrdSpel.BLL.Services;
using OrdSpel.Shared.DTOs;
using OrdSpel.Shared.GameDTOs;
using System.Security.Claims;

namespace OrdSpel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GameController : ControllerBase, IGameController
    {
        private readonly IGameService _gameService;
        private readonly IGameLobbyService _gameLobbyService;

        public GameController(IGameService gameService, IGameLobbyService gameLobbyService)
        {
            _gameService = gameService;
            _gameLobbyService = gameLobbyService;
        }

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
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpPut("end/{gameCode}")]
        public async Task<IActionResult> EndGame(string gameCode)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _gameService.EndGameAsync(gameCode, userId);
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(result.Data);
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
