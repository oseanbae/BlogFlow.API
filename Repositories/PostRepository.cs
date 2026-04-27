using BlogFlow.API.Data;
using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;
using BlogFlow.API.Queries;
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

        // READ — returns DTO directly via AsDTO() projection
        public async Task<PostReadDTO?> GetByIdAsync(Guid postId, bool includeDeleted = false)
        {
            var query = includeDeleted
                ? _context.Posts.IgnoreQueryFilters()
                : _context.Posts.AsQueryable();

            return await query
                .Where(p => p.Id == postId)
                .AsDTO()
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<PostReadDTO> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            Guid? authorId,
            Guid? categoryId,
            Guid? tagId,
            bool includeDeleted = false)
        {
            var query = includeDeleted
                ? _context.Posts.IgnoreQueryFilters()
                : _context.Posts.AsQueryable();

            if (authorId.HasValue)
                query = query.Where(p => p.AuthorId == authorId.Value);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (tagId.HasValue)
                query = query.Where(p => p.PostTags.Any(pt => pt.TagId == tagId.Value));

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsDTO()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<PostReadDTO> Items, int TotalCount)> SearchAsync(
            string keyword,
            int page,
            int pageSize,
            bool includeDeleted = false)
        {
            var query = includeDeleted
                ? _context.Posts.IgnoreQueryFilters()
                : _context.Posts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    p.Title.Contains(keyword) ||
                    p.Body.Contains(keyword));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsDTO()
                .ToListAsync();

            return (items, totalCount);
        }

        // WRITE — returns tracked Post entity for domain method calls in service
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
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Post post)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}