using BlogFlow.API.Data;
using BlogFlow.API.Models;
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
        public async Task<Comment?> GetTrackedByIdAsync(Guid id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}