using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryReadDTO> CreateCategoryAsync(CategoryCreateDTO dto, UserContext user);
        Task<CategoryReadDTO> RenameCategoryAsync(Guid categoryId, string newName, UserContext user);
        Task<IEnumerable<CategoryReadDTO>> GetCategoriesAsync();
        Task<CategoryReadDTO?> GetCategoryByIdAsync(Guid id);

    }
}
