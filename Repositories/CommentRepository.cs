using BlogFlow.API.Data;
using BlogFlow.API.Domain.Entities;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Comment> GetQueryable()
            => _context.Comments.AsNoTracking();

        public IQueryable<Comment> GetCommentsByPostQuery(Guid postId)
        {
            return _context.Comments
                .Where(c => c.PostId == postId)
                .AsNoTracking();
        }

        public async Task<Comment?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task AddAsync(Comment comment, CancellationToken cancellationToken)
        {
            await _context.Comments.AddAsync(comment, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}