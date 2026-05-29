using BlogFlow.API.Domain.Entities;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface IPostRepository
    {
        IQueryable<Post> GetPostsQuery(bool includeDeleted = false);

        Task<Post?> GetTrackedByIdAsync(Guid postId, bool includeDeleted = false, CancellationToken cancellationToken = default);

        Task AddAsync(Post post, CancellationToken cancellationToken = default);
        Task DeleteAsync(Post post);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}