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

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}/words")]
        public async Task<IActionResult> GetCategoriesContent(int id)
        {
            var words = await _categoryService.GetWordsByCategoryIdAsync(id);
            if (words == null || !words.Any())
            {
                return NotFound("Category not found");
            }

            return Ok(words);
        }
    }
}
