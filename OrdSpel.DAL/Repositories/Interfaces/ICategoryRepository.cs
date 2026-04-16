using OrdSpel.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<List<Word>> GetWordsByCategoryIdAsync(int id);
    }
}
