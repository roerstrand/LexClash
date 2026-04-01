using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdSpel.API.Interfaces;
using OrdSpel.BLL.Interfaces;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Repositories;

namespace OrdSpel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase, ICategoryController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet] //endpoint för att hämta categories
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}/words")] //endpoint för att hämta orden i en specifik category
        public async Task<IActionResult> GetCategoriesContent(int id)
        {
            var words = await _categoryService.GetWordsByCategoryIdAsync(id);
            if (words == null || !words.Any())
            {
                return NotFound("Kategorin finns inte");
            }

            return Ok(words);
        }
    }
}
