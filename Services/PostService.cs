using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;
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

        // CREATE
        public async Task<PostReadDTO> CreatePostAsync(PostCreateDTO dto, Guid authorId)
        {
            if (authorId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid user.");

            var post = new Post(dto.Title, dto.Body, authorId, dto.CategoryId);

            if (dto.TagIds != null && dto.TagIds.Any())
                post.SetTags(dto.TagIds);

            await _postRepo.AddAsync(post);

            // Re-fetch via repo to get full DTO projection with navigations
            return await _postRepo.GetByIdAsync(post.Id, includeDeleted: false)
                ?? throw new KeyNotFoundException("Post not found after creation.");
        }

        // GET ALL (ADMIN OR PUBLIC)
        public async Task<PaginatedPostResultDTO> GetAllPostsAsync(
            int page,
            int pageSize,
            bool isAdmin)
        {
            var (items, totalCount) = await _postRepo.GetPagedAsync(
                page,
                pageSize,
                authorId: null,
                categoryId: null,
                tagId: null,
                includeDeleted: isAdmin
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
            bool isAdmin)
        {
            return await _postRepo.GetByIdAsync(postId, isAdmin)
                ?? throw new KeyNotFoundException("Post not found.");
        }

        // GET PUBLISHED
        public async Task<PaginatedPostResultDTO> GetPublishedPostsAsync(
            int page,
            int pageSize,
            Guid? categoryId)
        {
            var (items, totalCount) = await _postRepo.GetPagedAsync(
                page,
                pageSize,
                authorId: null,
                categoryId: categoryId,
                tagId: null,
                includeDeleted: false
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
            UserRole requesterRole)
        {
            var (items, totalCount) = await _postRepo.GetPagedAsync(
                page,
                pageSize,
                authorId: null,
                categoryId: null,
                tagId: tagId,
                includeDeleted: requesterRole == UserRole.Admin
            );

            return new PaginatedPostResultDTO
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // SEARCH
        public async Task<PaginatedPostResultDTO> SearchPostsAsync(
            string keyword,
            int page,
            int pageSize,
            UserRole requesterRole)
        {
            var (items, totalCount) = await _postRepo.SearchAsync(
                keyword,
                page,
                pageSize,
                includeDeleted: requesterRole == UserRole.Admin
            );

            return new PaginatedPostResultDTO
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // UPDATE
        public async Task<PostReadDTO> UpdatePostAsync(
            Guid postId,
            PostUpdateDTO dto,
            Guid requesterId,
            bool isAdmin)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: isAdmin)
                ?? throw new KeyNotFoundException("Post not found.");

            if (!isAdmin && post.AuthorId != requesterId)
                throw new UnauthorizedAccessException("Not allowed.");

            if (!isAdmin && post.DeletedAt != null)
                throw new UnauthorizedAccessException("Cannot modify a deleted post.");

            post.Update(
                dto.Title ?? post.Title,
                dto.Body ?? post.Body,
                dto.CategoryId ?? post.CategoryId
            );

            if (dto.TagIds != null)
                post.SetTags(dto.TagIds);

            await _postRepo.SaveChangesAsync();

            // Re-fetch to return fully projected DTO
            return await _postRepo.GetByIdAsync(postId, includeDeleted: isAdmin)
                ?? throw new KeyNotFoundException("Post not found after update.");
        }

        // SOFT DELETE
        public async Task SoftDeletePostAsync(
            Guid postId,
            Guid requesterId,
            bool isAdmin)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: isAdmin)
                ?? throw new KeyNotFoundException("Post not found.");

            if (!isAdmin && post.AuthorId != requesterId)
                throw new UnauthorizedAccessException("Not allowed.");

            post.SoftDelete();

            await _postRepo.SaveChangesAsync();
        }

        // RESTORE
        public async Task RestorePostAsync(
            Guid postId,
            UserRole requesterRole)
        {
            if (requesterRole != UserRole.Admin)
                throw new UnauthorizedAccessException("Only admins can restore posts.");

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true)
                ?? throw new KeyNotFoundException("Post not found.");

            post.Restore();

            await _postRepo.SaveChangesAsync();
        }

        // HARD DELETE
        public async Task HardDeletePostAsync(
            Guid postId,
            UserRole requesterRole)
        {
            if (requesterRole != UserRole.Admin)
                throw new UnauthorizedAccessException("Only admins can hard delete posts.");

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true)
                ?? throw new KeyNotFoundException("Post not found.");

            await _postRepo.DeleteAsync(post);
        }
    }
}