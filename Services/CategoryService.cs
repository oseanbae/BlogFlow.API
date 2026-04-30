using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
namespace BlogFlow.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<CategoryReadDTO> CreateCategoryAsync(
            CategoryCreateDTO dto,
            UserContext user)
        {
            ValidateRequestSync(dto.Name, user);

            if (await _repo.ExistsByNameAsync(dto.Name, null))
                throw new InvalidOperationException("Category already exists.");

            var category = new Category(dto.Name);

            await _repo.CreateCategoryAsync(category);

            return new CategoryReadDTO
            {
                Id = category.Id,
                Name = category.DisplayName
            };
        }

        public async Task<CategoryReadDTO> RenameCategoryAsync(
            Guid categoryId,
            string newName,
            UserContext user)
        {
            ValidateRequestSync(newName, user);

            if (await _repo.ExistsByNameAsync(newName, categoryId))
                throw new InvalidOperationException("Category already exists.");

            await _repo.RenameCategoryAsync(categoryId, newName);

            var updated = await _repo.GetCategoryByIdAsync(categoryId);
            return updated!;
        }

        public Task<IEnumerable<CategoryReadDTO>> GetCategoriesAsync()
            => _repo.GetAllCategoriesAsync();

        public Task<CategoryReadDTO?> GetCategoryByIdAsync(Guid id)
            => _repo.GetCategoryByIdAsync(id);

        private static void ValidateRequestSync(string name, UserContext user)
        {
            if (!user.IsAdmin)
                throw new UnauthorizedAccessException("Invalid user.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");
        }
    }
}