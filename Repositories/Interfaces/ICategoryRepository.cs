namespace BlogFlow.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetCategoriesQuery();
        IQueryable<Category> GetCategoryQuery(Guid id);

        Task CreateCategoryAsync(Category category);
        Task RenameCategoryAsync(Guid id, string newName);
        Task SaveChangesAsync();
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
    }
}