using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Helper;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using System.Security.Claims;

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

            var created = await _postRepo.GetByIdWithDetailsAsync(post.Id);
            return MappingHelper.PostToDTO(created);
        }

        // GET ALL (ADMIN OR PUBLIC WITH FILTER)
        public async Task<PaginatedPostResultDTO> GetAllPostsAsync(
            int page,
            int pageSize,
            ClaimsPrincipal user)
        {
            bool ignoreSoftDelete = user?.IsInRole("Admin") == true;

            var (items, totalCount) = await _postRepo.GetPagedAsync(
                page,
                pageSize,
                authorId: null,
                categoryId: null,
                tagId: null,
                ignoreSoftDelete
            );

            return new PaginatedPostResultDTO
            {
                Items = items.Select(MappingHelper.PostToDTO).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // GET BY ID
        public async Task<PostReadDTO> GetPostByIdAsync(
        Guid postId,
        ClaimsPrincipal user)
        {
            bool isAdmin = user?.IsInRole("Admin") == true;

            var post = await _postRepo.GetByIdAsync(postId, isAdmin);

            if (post == null)
                throw new KeyNotFoundException("Post not found");

            return MappingHelper.PostToDTO(post);
        }

        // GET BY TAG
        public async Task<PaginatedPostResultDTO> GetPostsByTagAsync(
            Guid tagId,
            int page,
            int pageSize,
            Guid requesterId,
            UserRole requesterRole)
        {
            var (items, totalCount) = await _postRepo.GetPagedAsync(
                page,
                pageSize,
                null,
                null,
                tagId,
                ignoreSoftDelete: requesterRole == UserRole.Admin
            );

            return new PaginatedPostResultDTO
            {
                Items = items.Select(MappingHelper.PostToDTO).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
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
                null,
                categoryId,
                null,
                ignoreSoftDelete: false
            );

            return new PaginatedPostResultDTO
            {
                Items = items.Select(MappingHelper.PostToDTO).ToList(),
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
            Guid requesterId,
            UserRole requesterRole)
        {
            var includeDeleted = requesterRole == UserRole.Admin;

            var (items, totalCount) = await _postRepo.SearchAsync(
                keyword,
                page,
                pageSize,
                includeDeleted
            );

            return new PaginatedPostResultDTO
            {
                Items = items.Select(MappingHelper.PostToDTO).ToList(),
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
            UserRole requesterRole)
        {
            var post = await _postRepo.GetByIdWithDetailsAsync(postId)
                ?? throw new KeyNotFoundException("Post not found.");

            // 🔐 Authorization FIRST
            if (requesterRole != UserRole.Admin && post.AuthorId != requesterId)
                throw new UnauthorizedAccessException("Not allowed.");

            if (post.DeletedAt != null && requesterRole != UserRole.Admin)
                throw new UnauthorizedAccessException("Cannot modify deleted post.");

            // ✏️ Apply updates
            post.Update(
                dto.Title ?? post.Title,
                dto.Body ?? post.Body,
                dto.CategoryId ?? post.CategoryId
            );

            if (dto.TagIds != null)
            {
                post.SetTags(dto.TagIds);
            }

            await _postRepo.SaveChangesAsync();

            return MappingHelper.PostToDTO(post);
        }

        // SOFT DELETE
        public async Task SoftDeletePostAsync(
            Guid postId,
            Guid requesterId,
            UserRole requesterRole)
        {
            var post = await _postRepo.GetByIdAsync(postId, includeDeleted: requesterRole == UserRole.Admin);

            if (requesterRole != UserRole.Admin && post.AuthorId != requesterId)
                throw new UnauthorizedAccessException("Not allowed.");

            post.SoftDelete();

            await _postRepo.SaveChangesAsync();
        }

        // RESTORE
        public async Task RestorePostAsync(
            Guid postId,
            Guid requesterId,
            UserRole requesterRole)
        {
            if (requesterRole != UserRole.Admin)
                throw new UnauthorizedAccessException();

            var post = await _postRepo.GetByIdAsync(postId, includeDeleted: true);

            post.Restore();

            await _postRepo.SaveChangesAsync();
        }

        // HARD DELETE
        public async Task HardDeletePostAsync(
            Guid postId,
            Guid requesterId,
            UserRole requesterRole)
        {
            if (requesterRole != UserRole.Admin)
                throw new UnauthorizedAccessException();

            var post = await _postRepo.GetByIdWithDetailsAsync(postId)
                ?? throw new KeyNotFoundException("Post not found.");

            await _postRepo.DeleteAsync(post);
        }
    }
}