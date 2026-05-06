using BlogFlow.API.DTOs;
using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;
using BlogFlow.API.Queries;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ITagRepository _tagRepo;
        public PostService(IPostRepository repo, ICategoryRepository categoryRepo, ITagRepository tagRepo)
        {
            _postRepo = repo;
            _categoryRepo = categoryRepo;
            _tagRepo = tagRepo;
        }

        // --- READ OPERATIONS ---
        public async Task<PostReadDTO> GetPostByIdAsync(Guid postId, UserContext user)
        {
            var postDto = await _postRepo.GetPostsQuery(includeDeleted: user.IsAdmin)
                .Where(p => p.Id == postId)
                .AsDTO()
                .FirstOrDefaultAsync();

            return postDto ?? throw new KeyNotFoundException("Post not found.");
        }
        public async Task<PaginatedResultDTO<PostReadDTO>> GetPostsAsync(
            PostQueryParams p,
            UserContext user)
        {
            var query = _postRepo.GetPostsQuery(includeDeleted: user.IsAdmin);

            query = ApplyFilters(query, p);

            return await ExecutePagedQueryAsync(query, p.Page, p.PageSize);
        }

        // --- WRITE OPERATIONS ---

        public async Task<PostReadDTO> CreatePostAsync(PostCreateDTO dto, UserContext user)
        {
            if (!user.IsAuthenticated)
                throw new UnauthorizedAccessException("Authentication required.");

            if (!user.IsAuthor && !user.IsAdmin)
                throw new UnauthorizedAccessException("Only authors or admins can create posts.");

            var categoryExists = await _categoryRepo.GetCategoryQuery(dto.CategoryId).AnyAsync();
            if (!categoryExists)
                throw new KeyNotFoundException("Invalid category.");

            await ValidateTagsExistAsync(dto.TagIds);

            var post = new Post(dto.Title, dto.Body, user.UserId, dto.CategoryId);

            post.SetTags(dto.TagIds ?? []);

            await _postRepo.AddAsync(post);
            await _postRepo.SaveChangesAsync();

            return await GetPostByIdAsync(post.Id, user);
        }

        public async Task<PostReadDTO> UpdatePostAsync(Guid postId, PostUpdateDTO dto, UserContext user)
        {
            if (!user.IsAuthenticated)
                throw new UnauthorizedAccessException("Authentication required.");

            if (!user.IsAuthor && !user.IsAdmin)
                throw new UnauthorizedAccessException("Only authors or admins can update posts.");

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin)
                ?? throw new KeyNotFoundException("Post not found.");

            if (!user.IsAdmin && post.AuthorId != user.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            if (dto.CategoryId.HasValue)
            {
                if (dto.CategoryId.Value == Guid.Empty)
                    throw new ArgumentException("Invalid category.");

                var catExists = await _categoryRepo
                    .GetCategoryQuery(dto.CategoryId.Value)
                    .AnyAsync();

                if (!catExists)
                    throw new KeyNotFoundException("Invalid category.");
            }

            await ValidateTagsExistAsync(dto.TagIds);

            post.Update(
                dto.Title ?? post.Title,
                dto.Body ?? post.Body,
                dto.CategoryId ?? post.CategoryId
            );

            if (dto.TagIds != null)
                post.SetTags(dto.TagIds);

            await _postRepo.SaveChangesAsync();

            return await GetPostByIdAsync(post.Id, user);
        }

        public async Task SoftDeletePostAsync(Guid postId, UserContext user)
        {
            if (!user.IsAuthenticated)
                throw new UnauthorizedAccessException("Authentication required.");

            if (!user.IsAuthor && !user.IsAdmin)
                throw new UnauthorizedAccessException("Only authors or admins can delete posts.");

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin)
                ?? throw new KeyNotFoundException("Post not found.");

            if (!user.IsAdmin && post.AuthorId != user.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            post.SoftDelete();
            await _postRepo.SaveChangesAsync();
        }

        public async Task RestorePostAsync(Guid postId, UserContext user)
        {
            if (!user.IsAuthenticated)
                throw new UnauthorizedAccessException("Authentication required.");

            if (!user.IsAdmin)
                throw new UnauthorizedAccessException("Only admin can restore posts.");

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true)
                ?? throw new KeyNotFoundException("Post not found.");

            post.Restore();
            await _postRepo.SaveChangesAsync();
        }

        public async Task HardDeletePostAsync(Guid postId, UserContext user)
        {
            if (!user.IsAuthenticated)
                throw new UnauthorizedAccessException("Authentication required.");

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
            var pagedResult = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToPaginatedResultAsync(page, pageSize);

            return new PaginatedResultDTO<PostReadDTO>
            {
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                Items = pagedResult.Items.AsDTO().ToList()
            };
        }

        private async Task ValidateTagsExistAsync(IEnumerable<Guid>? tagIds)
        {
            if (tagIds == null || !tagIds.Any()) return;

            var uniqueTagIds = tagIds.Distinct().ToList();

            var validTagCount = await _tagRepo.GetTagsQuery()
                .Where(t => uniqueTagIds.Contains(t.Id))
                .CountAsync();

            if (validTagCount != uniqueTagIds.Count)
            {
                throw new KeyNotFoundException("One or more tags are invalid.");
            }
        }

        private static IQueryable<Post> ApplyFilters(IQueryable<Post> query, PostQueryParams p)
        {
            if (p.CategoryId is Guid catId)
                query = query.Where(post => post.CategoryId == catId);

            if (p.TagId is Guid tagId)
                query = query.Where(post => post.PostTags.Any(pt => pt.TagId == tagId));

            if (!string.IsNullOrWhiteSpace(p.Keyword))
                query = query.Where(post =>
                    post.Title.Contains(p.Keyword) ||
                    post.Body.Contains(p.Keyword));

            return query;
        }
    }
}