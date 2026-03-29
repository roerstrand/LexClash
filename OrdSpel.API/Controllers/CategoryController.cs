using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Data;

namespace OrdSpel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet] //enpoint för att hämta categories
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id}/words")] //endpoint för att hämta orden i en specifik category
        public async Task<IActionResult> GetCategoriesContent(int id)
        {
            var words = await _context.Words.Where(w => w.CategoryId == id).ToListAsync();

            return Ok(words);
        }
    }
}
