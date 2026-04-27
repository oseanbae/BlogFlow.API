using BlogFlow.API.Data;
using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using BlogFlow.API.Queries;

namespace BlogFlow.API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryReadDTO>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .AsDTO()
                .ToListAsync();
        }

        public async Task<CategoryReadDTO?> GetCategoryByIdAsync(Guid id)
        {
            return await _context.Categories
                   .AsDTO()
                   .FirstOrDefaultAsync(c => c.Id == id); 
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                await _context.SaveChangesAsync();
            }
        }
    }
}
