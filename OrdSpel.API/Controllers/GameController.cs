using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OrdSpel.API.Hubs;
using OrdSpel.API.Interfaces;
using OrdSpel.BLL.Interfaces;
using OrdSpel.BLL.Services;
using OrdSpel.Shared.DTOs;
using OrdSpel.Shared.GameDTOs;
using OrdSpel.Shared.Enums;
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
        private readonly IGameStatusService _gameStatusService;
        private readonly IHubContext<GameHub> _hubContext;

        public GameController(IGameService gameService, IGameLobbyService gameLobbyService, IGameStatusService gameStatusService, IHubContext<GameHub> hubContext)
        {
            _gameService = gameService;
            _gameLobbyService = gameLobbyService;
            _gameStatusService = gameStatusService;
            _hubContext = hubContext;
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

            await _hubContext.Clients.Group(dto.GameCode).SendAsync("LobbyUpdated", dto.GameCode);

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

        [HttpGet("{code}/result")]
        public async Task<ActionResult<GameResultDto>> GetResult(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Game code is required.");
            }

            var result = await _gameService.GetGameResultAsync(code);
            if (result is null)
            {
                return NotFound("Game session was not found.");
            }

            if (result.Status != GameStatus.GameFinished)
            {
                return Conflict("Game is not finished yet.");
            }

            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveGame()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _gameService.GetActiveGameByUserAsync(userId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetGameHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _gameService.GetGameHistoryAsync(userId);
            return Ok(result);
        }

        [HttpDelete("{gameCode}")]
        public async Task<IActionResult> DeleteGame(string gameCode)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _gameService.DeleteGameAsync(gameCode, userId);
            if (!result.Success)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpGet("{code}/status")]
        public async Task<ActionResult<GameStatusDto>> GetGameStatus(string code)
        {
            var result = await _gameStatusService.GetGameStatusAsync(code);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
