using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Models;
namespace BlogFlow.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task CreateCategoryAsync(Category category);
        Task RenameCategoryAsync(Guid id, string newName);
        Task<IEnumerable<CategoryReadDTO>> GetAllCategoriesAsync();
        Task<CategoryReadDTO?> GetCategoryByIdAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name);
    }
}