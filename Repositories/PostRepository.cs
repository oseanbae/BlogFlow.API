using BlogFlow.API.Data;
using BlogFlow.API.Models;
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

        public async Task<Post?> GetTrackedByIdAsync(Guid postId, bool includeDeleted = false)
        {
            var query = includeDeleted
                ? _context.Posts.IgnoreQueryFilters()
                : _context.Posts.AsQueryable();

            return await query
                .Include(p => p.PostTags)
                .FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
        }

        public async Task DeleteAsync(Post post)
        {
            _context.Posts.Remove(post);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}