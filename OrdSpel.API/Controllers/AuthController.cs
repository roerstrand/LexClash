using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdSpel.API.Services;
using OrdSpel.BLL.Services;
using OrdSpel.Shared.AuthDTOs;
using System.Security.Claims;

namespace OrdSpel.API.Controllers
{
    
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JwtService _jwtService;

        public AuthController(IAuthService authService, JwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto);
            if (user == null) return Unauthorized();

            var token = _jwtService.GenerateToken(user);
            return Ok(new TokenResponse { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.RegisterAsync(dto);

            if (user == null)
                return BadRequest("Något gick fel vid registrering.");

            var token = _jwtService.GenerateToken(user);
            return Ok(new TokenResponse { Token = token });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new { userId, username });
        }
    }
}
