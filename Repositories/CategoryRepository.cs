using BlogFlow.API.Data;
using BlogFlow.API.Helper;
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

        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task CreateCategoryAsync(Category category, CancellationToken cancellationToken)
            => await _context.Categories.AddAsync(category, cancellationToken);

        public Task SaveChangesAsync(CancellationToken cancellationToken)
            => _context.SaveChangesAsync(cancellationToken);

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var normalized = SlugHelper.Normalize(name);

            return await _context.Categories.AnyAsync(
                c =>
                    c.Name == normalized &&
                    (!excludeId.HasValue || c.Id != excludeId.Value),
                cancellationToken);
        }
    }
}