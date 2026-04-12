using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Models;
using OrdSpel.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<List<Word>> GetWordsByCategoryIdAsync(int id)
        {
            return await _context.Words.Where(w => w.CategoryId == id).ToListAsync();
        }
    }
}
