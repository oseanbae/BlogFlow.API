using BlogFlow.API.DTOs.Categories;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryReadDTO> CreateCategoryAsync(CategoryCreateDTO dto, CancellationToken cancellationToken);
        Task<CategoryReadDTO> RenameCategoryAsync(Guid categoryId, string newName, CancellationToken cancellationToken);
        Task<IEnumerable<CategoryReadDTO>> GetCategoriesAsync(CancellationToken cancellationToken);
        Task<CategoryReadDTO?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
