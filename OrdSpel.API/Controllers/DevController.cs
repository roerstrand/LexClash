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
        /// Rensar all testdata: alla spelsessioner och alla användare.
        /// Kategorier och ord behålls. Endast tillgänglig i Development-miljön.
        /// </summary>
        [HttpDelete("reset")]
        public async Task<IActionResult> Reset()
        {
            if (!_env.IsDevelopment())
                return NotFound();

            // GameTurns och GamePlayers tas bort via cascade när GameSessions tas bort
            await _appDb.GameSessions.ExecuteDeleteAsync();

            // Ta bort alla användare
            await _authDb.Users.ExecuteDeleteAsync();

            return Ok(new { message = "Reset complete. GameSessions, GamePlayers, GameTurns och Users borttagna." });
        }
    }
}
