using BlogFlow.API.Data;
using BlogFlow.API.Domain.Entities;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context;
        }

        // Returns raw IQueryable for the Service to project
        public IQueryable<Post> GetPostsQuery(bool includeDeleted = false)
        {
            return includeDeleted
                ? _context.Posts.IgnoreQueryFilters()
                : _context.Posts.AsQueryable();
        }

        public async Task<Post?> GetTrackedByIdAsync(Guid postId, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            var query = includeDeleted
                ? _context.Posts.IgnoreQueryFilters()
                : _context.Posts.AsQueryable();

            return await query
                .Include(p => p.PostTags)
                .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
        }

        public async Task AddAsync(Post post, CancellationToken cancellationToken = default)
        {
            await _context.Posts.AddAsync(post, cancellationToken);
        }

        public async Task DeleteAsync(Post post)
        {
            _context.Posts.Remove(post);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}