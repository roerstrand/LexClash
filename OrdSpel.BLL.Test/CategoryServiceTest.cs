using Moq;
using OrdSpel.BLL.Services;
using OrdSpel.DAL.Models;
using OrdSpel.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.BLL.Test
{
    public class CategoryServiceTest
    {
        private readonly Mock<ICategoryRepository> _mockRepo;
        private readonly CategoryService _service;

        public CategoryServiceTest()
        {
            _mockRepo = new Mock<ICategoryRepository>();
            _service = new CategoryService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllCategoriesAsync()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Animals" },
                new Category { Id = 2, Name = "Countries" },
                new Category { Id = 3, Name = "Fruits and Vegetables" }
            };

            _mockRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(categories);

            var result = await _service.GetAllAsync();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetWordsByCategoryIdFail()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(4))
                     .ReturnsAsync((Category?)null);

            var result = await _service.GetWordsByCategoryIdAsync(4);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetWordsByCategoryIdSuccess()
        {
            var category = new Category { Id = 1, Name = "Animals" };
            var words = new List<Word>
            {
                new Word { Id = 1, Text = "lejon", CategoryId = 1 },
                new Word { Id = 2, Text = "tiger", CategoryId = 1 }
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(category);
            _mockRepo.Setup(r => r.GetWordsByCategoryIdAsync(1))
                     .ReturnsAsync(words);

            var result = await _service.GetWordsByCategoryIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
