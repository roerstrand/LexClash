using Microsoft.AspNetCore.Mvc;
using OrdSpel.BLL.Services;
using OrdSpel.Shared.UserDtos;

namespace OrdSpel.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            //kollar att allt är korrekt ifyllt, annars skickas ett badrequest tillbaka
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //Skickar till BLL för att skapa en ny användare och få tillbaka en token
            var token = await _authService.RegisterAsync(dto);

            if (token == null)
                return BadRequest("Något gick fel vid registrering.");

            return Ok(new { token });
        }
    }
}
