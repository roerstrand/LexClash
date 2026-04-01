using OrdSpel.BLL.Interfaces;
using OrdSpel.DAL.Repositories.Interfaces;
using OrdSpel.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.BLL.Services
{
    public class WordService : IWordService
    {
        private readonly IWordRepository _wordRepository;

        public WordService(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }
        public async Task<List<WordDto>> GetAllAsync()
        {
            var words = await _wordRepository.GetAllAsync();

            return words.Select(w => new WordDto
            {
                Text = w.Text
            }).ToList();
        }
    }
}
