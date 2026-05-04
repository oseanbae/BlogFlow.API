using BlogFlow.API.Models;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface IPostRepository
    {
        // For Reading: Provides the base query for projection
        IQueryable<Post> GetPostsQuery(bool includeDeleted = false);

        // For Writing/Updating: Provides a tracked entity from the DB
        Task<Post?> GetTrackedByIdAsync(Guid postId, bool includeDeleted = false);

        // CRUD Operations
        Task AddAsync(Post post);
        Task DeleteAsync(Post post);
        Task SaveChangesAsync();
    }
}