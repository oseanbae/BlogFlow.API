using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BlogFlow.API.QueryExtensions;
using BlogFlow.API.Exceptions;

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
                throw new NotFoundException("Category", id);
        }

        public async Task<CategoryReadDTO> CreateCategoryAsync(CategoryCreateDTO dto)
        {
            ValidateRequestSync(dto.Name);

            if (await _repo.ExistsByNameAsync(dto.Name))
                throw new ConflictException($"Category '{dto.Name}' already exists.", "CATEGORY_ALREADY_EXISTS");

            var category = new Category(dto.Name);

            await _repo.CreateCategoryAsync(category);
            await _repo.SaveChangesAsync();

            return category.ToDTO();
        }

        public async Task<CategoryReadDTO> RenameCategoryAsync(Guid id, string newName)
        {
            ValidateRequestSync(newName);

            var category = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("Category", id);

            if (await _repo.ExistsByNameAsync(newName, id))
                throw new ConflictException($"A category with the name '{newName}' already exists.", "CATEGORY_NAME_CONFLICT");

            category.Rename(newName);

            await _repo.SaveChangesAsync();

            return category.ToDTO();
        }

        private static void ValidateRequestSync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BadRequestException("Category name is required.", "EMPTY_CATEGORY_NAME");
        }
    }
}