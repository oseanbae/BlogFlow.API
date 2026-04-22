using BlogFlow.API.Models;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface IPostRepository
    {
        // Write operations
        Task AddAsync(Post post);
        Task DeleteAsync(Post post); // hard delete only used by Admin
        Task<Post> GetByIdAsync(Guid postId, bool includeDeleted = false);
        Task SaveChangesAsync();
        Task<Post> GetByIdWithDetailsAsync(Guid postId);
        Task<IEnumerable<Post>> GetAllAsync(bool includeDeleted = false);
        Task<(IEnumerable<Post> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            Guid? authorId,
            Guid? categoryId,
            Guid? tagId,
            bool ignoreSoftDelete = false
        );

        Task<(IEnumerable<Post> Items, int TotalCount)> SearchAsync(
            string keyword,
            int page,
            int pageSize,
            bool includeDeleted = false
        );
    }
}