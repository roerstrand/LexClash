using OrdSpel.BLL.Interfaces;
using OrdSpel.DAL.Models;
using OrdSpel.DAL.Repositories.Interfaces;
using OrdSpel.Shared.DTOs;

namespace OrdSpel.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        public async Task<List<WordDto>?> GetWordsByCategoryIdAsync(int id)
        {
            var category = await _categoryRepository.GetWordsByCategoryIdAsync(id);
            if (category == null)
            {
                return null;
            }

            var words = await _categoryRepository.GetWordsByCategoryIdAsync(id);

            return words.Select(w => new WordDto
            {
                Text = w.Text
            }).ToList();
        }
    }
}
