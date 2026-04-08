using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OrdSpel.API.Hubs;
using OrdSpel.BLL.Interfaces;
using OrdSpel.Shared.DTOs;
using System.Security.Claims;

namespace OrdSpel.API.Controllers
{
    [Authorize] //måste vara inloggad för att kunna använda endpointen
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
            //hämta användarens id från jwt token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //om ingen användare hittas returneras unauthorixed
            if (userId == null)
            {
                return Unauthorized();
            }

            //ta emot response eller error från PlayTurnAsync-metoden i turnService
            var (response, error) = await _turnService.PlayTurnAsync(code, userId, dto);

            //returnera error om error inte är null 
            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            //skcika signalR-event efter lyckad turn
            await _hubContext.Clients.Group(code)
                .SendAsync("TurnUpdated", code);

            //eller returnera response om error är null
            return Ok(response);
        }
    }
}
