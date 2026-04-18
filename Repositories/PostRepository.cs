using BlogFlow.API.Data;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace BlogFlow.API.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        //Hard delete
        public async Task DeleteAsync(Post post)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            var postToUpdate = await _context.Posts
                .Include(p => p.PostTags)
                .FirstOrDefaultAsync(p => p.Id == post.Id && p.DeletedAt == null)
                ?? throw new KeyNotFoundException("Post not found.");

            postToUpdate.Update(post.Title, post.Body, post.CategoryId);

            if (post.PostTags != null)
            {
                postToUpdate.PostTags.Clear();
                foreach (var tag in post.PostTags)
                {
                    postToUpdate.PostTags.Add(tag);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Post>> GetAllAsync(bool includeDeleted = false)
        {
            var query = includeDeleted
                ? _context.Posts.IgnoreQueryFilters()
                : _context.Posts;

            return await query.ToListAsync();
        }

        public async Task<Post> GetByIdAsync(Guid postId, bool includeDeleted = false)
        {
            var query = includeDeleted
                ? _context.Posts.IgnoreQueryFilters()
                : _context.Posts;

            return await query
                .FirstOrDefaultAsync(p => p.Id == postId)
                ?? throw new KeyNotFoundException("Post not found.");
        }

        public async Task<(IEnumerable<Post> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, Guid? authorId, Guid? categoryId, bool includeDeleted = false)
        {

            var query = includeDeleted
                ? _context.Posts.IgnoreQueryFilters()
                : _context.Posts;

            if (authorId.HasValue)
                query = query.Where(p  => p.AuthorId == authorId.Value);
            if (categoryId.HasValue)
                query = query.Where(p  => p.CategoryId == categoryId.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<Post> Items, int TotalCount)> SearchAsync(
            string keyword, int page, int pageSize, bool includeDeleted = false)
        {
            var query = includeDeleted
                ? _context.Posts.IgnoreQueryFilters()
                : _context.Posts;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p =>
                        p.Title.Contains(keyword) ||
                        p.Body.Contains(keyword));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        
    }
}
