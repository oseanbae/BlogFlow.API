using BlogFlow.API.Data;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) => _context = context;

        public IQueryable<Category> GetCategoriesQuery()
            => _context.Categories.AsNoTracking();

        public IQueryable<Category> GetCategoryQuery(Guid id)
            => _context.Categories.AsNoTracking().Where(c => c.Id == id);

        public async Task CreateCategoryAsync(Category category)
            => await _context.Categories.AddAsync(category);

        public async Task RenameCategoryAsync(Guid id, string newName)
        {
            var category = await _context.Categories.FindAsync(id)
                ?? throw new KeyNotFoundException($"Category with ID {id} not found.");

            category.Rename(newName);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var normalized = Category.Normalize(name);

            return await _context.Categories.AnyAsync(c =>
                c.Name == normalized &&
                (!excludeId.HasValue || c.Id != excludeId.Value));
        }
    }
}