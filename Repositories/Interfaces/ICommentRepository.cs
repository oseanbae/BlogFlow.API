using BlogFlow.API.Domain.Entities;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        IQueryable<Comment> GetQueryable();
        IQueryable<Comment> GetCommentsByPostQuery(Guid postId);
        Task<Comment?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Comment comment, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}