using BlogFlow.API.Models;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        // Queries (Returns IQueryable for projection)
        IQueryable<Comment> GetQueryable();
        IQueryable<Comment> GetCommentsByPostQuery(Guid postId);

        // Actions (Returns Entities for state changes)
        Task<Comment?> GetTrackedByIdAsync(Guid id);
        Task AddAsync(Comment comment);
        Task SaveChangesAsync();
    }
}