using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BlogFlow.API.QueryExtensions;

namespace BlogFlow.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo) => _repo = repo;

        public async Task<IEnumerable<CategoryReadDTO>> GetCategoriesAsync()
        {
            return await _repo.GetCategoriesQuery()
                .AsDTO()
                .ToListAsync();
        }

        public async Task<CategoryReadDTO?> GetCategoryByIdAsync(Guid id)
        {
            return await _repo.GetCategoryQuery(id)
                .AsDTO()
                .FirstOrDefaultAsync() ??
                throw new KeyNotFoundException("Category not found");
        }

        public async Task<CategoryReadDTO> CreateCategoryAsync(CategoryCreateDTO dto, UserContext user)
        {
            ValidateRequestSync(dto.Name);

            if (await _repo.ExistsByNameAsync(dto.Name))
                throw new InvalidOperationException("Category already exists.");

            var category = new Category(dto.Name);

            await _repo.CreateCategoryAsync(category);
            await _repo.SaveChangesAsync();

            return await _repo.GetCategoryQuery(category.Id)
                .AsDTO()
                .FirstAsync();
        }

        public async Task<CategoryReadDTO> RenameCategoryAsync(Guid categoryId, string newName, UserContext user)
        {
            ValidateRequestSync(newName);

            if (await _repo.ExistsByNameAsync(newName, categoryId))
                throw new InvalidOperationException("A category with this name already exists.");

            await _repo.RenameCategoryAsync(categoryId, newName);
            await _repo.SaveChangesAsync();

            return await _repo.GetCategoryQuery(categoryId)
                .AsDTO()
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Updated category not found.");
        }

        private static void ValidateRequestSync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required.");
        }
    }
}