using Moq;
using OrdSpel.BLL.Services;
using OrdSpel.DAL.Models;
using OrdSpel.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.BLL.Test
{
    public class WordServiceTest
    {
        private readonly Mock<IWordRepository> _mockRepo;
        private readonly WordService _service;

        public WordServiceTest()
        {
            _mockRepo = new Mock<IWordRepository>();
            _service = new WordService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllWordsAsync()
        {
            var words = new List<Word>
            {
                new Word { Id = 1, Text = "lejon", CategoryId = 1 },
                new Word { Id = 2, Text = "tiger", CategoryId = 1 },
                new Word { Id = 3, Text = "apple", CategoryId = 3 }
            };

            _mockRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(words);

            var result = await _service.GetAllAsync();

            Assert.Equal(3, result.Count);
        }
    }
}
