namespace BlogFlow.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetCategoriesQuery();
        IQueryable<Category> GetCategoryQuery(Guid id);

        Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task CreateCategoryAsync(Category category, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    }
}