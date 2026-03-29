using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Data;

namespace OrdSpel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WordController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet] //enpoint för att hämta alla ord
        public async Task<IActionResult> GetAllWords()
        {
            var words = await _context.Words.ToListAsync();
            return Ok(words);
        }
        
        //endpoint för att hämta ord i en specifik kategori finns i CategoryController

    }
}
