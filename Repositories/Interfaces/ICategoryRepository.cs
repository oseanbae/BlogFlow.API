using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Models;
namespace BlogFlow.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
        Task<IEnumerable<CategoryReadDTO>> GetAllCategoriesAsync();
        Task<CategoryReadDTO?> GetCategoryByIdAsync(Guid id);
    }
}
