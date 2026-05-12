using BlogFlow.API.DTOs.Categories;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryReadDTO> CreateCategoryAsync(CategoryCreateDTO dto);
        Task<CategoryReadDTO> RenameCategoryAsync(Guid categoryId, string newName);
        Task<IEnumerable<CategoryReadDTO>> GetCategoriesAsync();
        Task<CategoryReadDTO?> GetCategoryByIdAsync(Guid id);

    }
}
