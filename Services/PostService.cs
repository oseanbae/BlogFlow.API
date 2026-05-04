using BlogFlow.API.DTOs;
using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;
using BlogFlow.API.Queries; // Ensure this contains AsDTO() and ApplyPagination()
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepo;

        public PostService(IPostRepository repo)
        {
            _postRepo = repo;
        }

        // --- READ OPERATIONS ---

        public async Task<PaginatedResultDTO<PostReadDTO>> GetPostsAsync(
            int page,
            int pageSize,
            Guid? categoryId,
            UserContext user)
        {
            var query = _postRepo.GetPostsQuery(user.IsAdmin);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            return await ExecutePagedQueryAsync(query, page, pageSize);
        }

        public async Task<PostReadDTO> GetPostByIdAsync(Guid postId, UserContext user)
        {
            var postDto = await _postRepo.GetPostsQuery(user.IsAdmin)
                .Where(p => p.Id == postId)
                .AsDTO()
                .FirstOrDefaultAsync();

            return postDto ?? throw new KeyNotFoundException("Post not found.");
        }

        public async Task<PaginatedResultDTO<PostReadDTO>> SearchPostsAsync(
            string keyword,
            int page,
            int pageSize,
            UserContext user)
        {
            var query = _postRepo.GetPostsQuery(user.IsAdmin);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.Title.Contains(keyword) || p.Body.Contains(keyword));
            }

            return await ExecutePagedQueryAsync(query, page, pageSize);
        }

        public async Task<PaginatedResultDTO<PostReadDTO>> GetPostsByTagAsync(
            Guid tagId,
            int page,
            int pageSize,
            UserContext user)
        {
            var query = _postRepo.GetPostsQuery(user.IsAdmin)
                .Where(p => p.PostTags.Any(pt => pt.TagId == tagId));

            return await ExecutePagedQueryAsync(query, page, pageSize);
        }

        // --- WRITE OPERATIONS ---

        public async Task<PostReadDTO> CreatePostAsync(PostCreateDTO dto, UserContext user)
        {
            var post = new Post(dto.Title, dto.Body, user.UserId, dto.CategoryId);

            if (dto.TagIds != null && dto.TagIds.Count != 0)
                post.SetTags(dto.TagIds);

            await _postRepo.AddAsync(post);
            await _postRepo.SaveChangesAsync(); // Commit transaction

            return post.ToDTO();
        }

        public async Task<PostReadDTO> UpdatePostAsync(Guid postId, PostUpdateDTO dto, UserContext user)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, user.IsAdmin)
                ?? throw new KeyNotFoundException("Post not found.");

            if (!user.IsAdmin && post.AuthorId != user.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            post.Update(
                dto.Title ?? post.Title,
                dto.Body ?? post.Body,
                dto.CategoryId ?? post.CategoryId
            );

            if (dto.TagIds != null)
                post.SetTags(dto.TagIds);

            await _postRepo.SaveChangesAsync();
            return post.ToDTO();
        }

        public async Task SoftDeletePostAsync(Guid postId, UserContext user)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, user.IsAdmin)
                ?? throw new KeyNotFoundException("Post not found.");

            if (!user.IsAdmin && post.AuthorId != user.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            post.SoftDelete();
            await _postRepo.SaveChangesAsync();
        }

        public async Task RestorePostAsync(Guid postId, UserContext user)
        {
            if (!user.IsAdmin)
                throw new UnauthorizedAccessException("Only admin can restore posts.");

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true)
                ?? throw new KeyNotFoundException("Post not found.");

            post.Restore();
            await _postRepo.SaveChangesAsync();
        }

        public async Task HardDeletePostAsync(Guid postId, UserContext user)
        {
            if (!user.IsAdmin)
                throw new UnauthorizedAccessException("Only admin can hard delete posts.");

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true)
                ?? throw new KeyNotFoundException("Post not found.");

            await _postRepo.DeleteAsync(post);
            await _postRepo.SaveChangesAsync();
        }

        // --- PRIVATE HELPER ---

        private static async Task<PaginatedResultDTO<PostReadDTO>> ExecutePagedQueryAsync(
            IQueryable<Post> query, int page, int pageSize)
        {
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .AsDTO()
                .ApplyPagination(page, pageSize)
                .ToListAsync();

            return new PaginatedResultDTO<PostReadDTO>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}