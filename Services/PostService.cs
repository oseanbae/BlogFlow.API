using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;
using BlogFlow.API.Queries;
using BlogFlow.API.Services.Interfaces;

namespace BlogFlow.API.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepo;

        public PostService(IPostRepository repo)
        {
            _postRepo = repo;
        }

        // GET ALL OR PUBLISHED
        public async Task<PaginatedPostResultDTO> GetPostsAsync(
            int page,
            int pageSize,
            Guid? categoryId,
            UserContext user)
        {
            var (items, totalCount) = await _postRepo.GetPagedAsync(
                page,
                pageSize,
                authorId: null,
                categoryId: categoryId,
                tagId: null,
                includeDeleted: user.IsAdmin
            );

            return new PaginatedPostResultDTO
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // GET BY ID
        public async Task<PostReadDTO> GetPostByIdAsync(
            Guid postId,
            UserContext user)
        {
            return await _postRepo.GetByIdAsync(postId, includeDeleted: user.IsAdmin)
                ?? throw new KeyNotFoundException("Post not found.");
        }

        // SEARCH
        public async Task<PaginatedPostResultDTO> SearchPostsAsync(
            string keyword,
            int page,
            int pageSize,
            UserContext user)
        {
            var (items, totalCount) = await _postRepo.SearchAsync(
                keyword,
                page,
                pageSize,
                includeDeleted: user.IsAdmin
            );

            return new PaginatedPostResultDTO
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // GET BY TAG
        public async Task<PaginatedPostResultDTO> GetPostsByTagAsync(
            Guid tagId,
            int page,
            int pageSize,
            UserContext user)
        {
            var (items, totalCount) = await _postRepo.GetPagedAsync(
                page,
                pageSize,
                authorId: null,
                categoryId: null,
                tagId: tagId,
                includeDeleted: user.IsAdmin
            );

            return new PaginatedPostResultDTO
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // CREATE
        public async Task<PostReadDTO> CreatePostAsync(
            PostCreateDTO dto,
            UserContext user)
        {
            if (!user.IsAuthenticated || !user.UserId.HasValue)
                throw new UnauthorizedAccessException("Invalid user.");

            var post = new Post(dto.Title, dto.Body, user.UserId.Value, dto.CategoryId);

            if (dto.TagIds != null && dto.TagIds.Any())
                post.SetTags(dto.TagIds);

            await _postRepo.AddAsync(post);

            return post.ToDTO();
        }

        // UPDATE
        public async Task<PostReadDTO> UpdatePostAsync(
            Guid postId,
            PostUpdateDTO dto,
            UserContext user)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin)
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

        // SOFT DELETE
        public async Task SoftDeletePostAsync(
            Guid postId,
            UserContext user)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin)
                ?? throw new KeyNotFoundException("Post not found.");

            if (!user.IsAdmin && post.AuthorId != user.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            post.SoftDelete();

            await _postRepo.SaveChangesAsync();
        }

        // RESTORE
        public async Task RestorePostAsync(
            Guid postId,
            UserContext user)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true)
                ?? throw new KeyNotFoundException("Post not found.");
            if (!user.IsAdmin)
                throw new UnauthorizedAccessException("Only admin can restore posts.");

            post.Restore();

            await _postRepo.SaveChangesAsync();
        }

        // HARD DELETE
        public async Task HardDeletePostAsync(
            Guid postId,
            UserContext user)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true)
                ?? throw new KeyNotFoundException("Post not found.");

            if (!user.IsAdmin)
                throw new UnauthorizedAccessException("Only admin can hard delete posts.");

            await _postRepo.DeleteAsync(post);
        }
    }
}