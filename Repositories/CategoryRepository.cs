using BlogFlow.API.Data;
using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Queries;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task RenameCategoryAsync(Guid id, string newName)
        {
            var category = await _context.Categories.FindAsync(id)
                ?? throw new KeyNotFoundException($"Category with ID {id} not found.");

            category.Rename(newName);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var normalized = Category.Normalize(name);

            return _context.Categories.AnyAsync(c =>
                c.Name == normalized &&
                (!excludeId.HasValue || c.Id != excludeId.Value));
        }
    }
}
