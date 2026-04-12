using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OrdSpel.API.Hubs;
using OrdSpel.BLL.Interfaces;
using OrdSpel.Shared.DTOs;
using System.Security.Claims;

namespace OrdSpel.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/games/{code}/turns")]
    public class TurnController : ControllerBase
    {
        private readonly ITurnService _turnService;
        private readonly IHubContext<GameHub> _hubContext;

        public TurnController(ITurnService turnService, IHubContext<GameHub> hubContext)
        {
            _turnService = turnService;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostTurn(string code, [FromBody] TurnRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var (response, error) = await _turnService.PlayTurnAsync(code, userId, dto);

            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            await _hubContext.Clients.Group(code)
                .SendAsync("TurnUpdated", code);

            return Ok(response);
        }
    }
}
