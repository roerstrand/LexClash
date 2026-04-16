using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Data;

namespace OrdSpel.API.Controllers
{
    [ApiController]
    [Route("api/dev")]
    public class DevController : ControllerBase
    {
        private readonly AppDbContext _appDb;
        private readonly AuthDbContext _authDb;
        private readonly IWebHostEnvironment _env;

        public DevController(AppDbContext appDb, AuthDbContext authDb, IWebHostEnvironment env)
        {
            _appDb = appDb;
            _authDb = authDb;
            _env = env;
        }

        /// <summary>
        /// Resets all test data: game sessions and users. Categories and words are kept. Only available in Development.
        /// </summary>
        [HttpDelete("reset")]
        public async Task<IActionResult> Reset()
        {
            if (!_env.IsDevelopment())
                return NotFound();

            await _appDb.GameSessions.ExecuteDeleteAsync();
            await _authDb.Users.ExecuteDeleteAsync();

            return Ok(new { message = "Reset complete. GameSessions, GamePlayers, GameTurns and Users removed." });
        }
    }
}
