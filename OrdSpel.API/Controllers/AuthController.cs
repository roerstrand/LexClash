using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrdSpel.API.Interfaces;
using OrdSpel.BLL.Interfaces;
using OrdSpel.Shared.AuthDTOs;
using System.Security.Claims;

namespace OrdSpel.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase, IAuthController
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(IAuthService authService, SignInManager<IdentityUser> signInManager)
        {
            _authService = authService;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto);
            if (user == null) return Unauthorized();

            // Sätter auth-cookien i svaret – webbläsaren lagrar den automatiskt
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok(new { userId = user.Id, username = user.UserName });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);
            if (!result.Success)
                return BadRequest(result.Error);

            // Logga in direkt efter registrering
            await _signInManager.SignInAsync(result.Data!, isPersistent: false);
            return Ok(new { userId = result.Data!.Id, username = result.Data.UserName });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new { userId, username });
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var success = await _authService.DeleteAsync(userId);
            if (!success) return BadRequest("Något gick fel vid borttagning av konto.");

            await _signInManager.SignOutAsync();
            return Ok("Kontot har tagits bort.");
        }
    }
}
