using BlogFlow.API.Models;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<Post> GetByIdAsync(Guid postId, bool includeDeleed = false);
        Task<IEnumerable<Post>> GetAllAsync(bool includeDeleted = false);

        Task<(IEnumerable<Post> Items, int TotalCount)> GetPagedAsync
            (
                int page,
                int pageSize,
                Guid? authorId,
                Guid? categoryId,
                bool includeDeleted = false
            );

        Task<(IEnumerable<Post> Items, int TotalCount)> SearchAsync
            (
                string keyword,
                int page,
                int pageSize,
                bool includeDeleted = false
            );

        // Write operations
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Post post); // hard delete only used by Admin

    }
}
