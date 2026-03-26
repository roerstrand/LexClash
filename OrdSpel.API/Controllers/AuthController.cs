using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdSpel.API.Services;
using OrdSpel.BLL.Services;
using OrdSpel.Shared.UserDTOs;

namespace OrdSpel.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtService _jwtService;

        public AuthController(AuthService authService, JwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto);
            if (user == null) return Unauthorized();

            var token = _jwtService.GenerateToken(user);
            return Ok(new TokenResponse { Token = token });
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
