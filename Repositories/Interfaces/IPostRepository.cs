using BlogFlow.API.Domain.Entities;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface IPostRepository
    {
        // For Reading: Provides the base query for projection
        IQueryable<Post> GetPostsQuery(bool includeDeleted = false);

        // For Writing/Updating: Provides a tracked entity from the DB
        Task<Post?> GetTrackedByIdAsync(Guid postId, bool includeDeleted = false, CancellationToken cancellationToken = default);

        // CRUD Operations
        Task AddAsync(Post post, CancellationToken cancellationToken = default);
        Task DeleteAsync(Post post);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}