using BlogFlow.API.DTOs.Common;
using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Exceptions;
using BlogFlow.API.Models;
using BlogFlow.API.QueryExtensions;
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
        private readonly ILogger<PostService> _logger;

        public PostService(
            IPostRepository postRepo,
            ICategoryRepository categoryRepo,
            ITagRepository tagRepo,
            ILogger<PostService> logger)
        {
            _postRepo = postRepo;
            _categoryRepo = categoryRepo;
            _tagRepo = tagRepo;
            _logger = logger;
        }

        // --- READ OPERATIONS ---
        public async Task<PostReadDTO> GetPostByIdAsync(
            Guid postId,
            UserContext user,
            CancellationToken cancellationToken)
        {
            var postDto = await _postRepo.GetPostsQuery(includeDeleted: user.IsAdmin)
                .Where(p => p.Id == postId)
                .AsDTO()
                .FirstOrDefaultAsync(cancellationToken);

            return postDto ?? throw new NotFoundException("Post", postId);
        }

        public async Task<PaginatedResultDTO<PostReadDTO>> GetPostsAsync(
            PostQueryParams p,
            UserContext user,
            CancellationToken cancellationToken)
        {
            var query = _postRepo.GetPostsQuery(includeDeleted: user.IsAdmin);

            query = ApplyFilters(query, p);
            query = ApplySorting(query);

            _logger.LogInformation(
                "Fetching posts with filters for user {UserId}",
                user.UserId);

            return await ExecutePagedQueryAsync(query, p.Page, p.PageSize, cancellationToken);
        }

        // --- WRITE OPERATIONS ---

        public async Task<PostReadDTO> CreatePostAsync(
            PostCreateDTO dto,
            UserContext user,
            CancellationToken cancellationToken)
        {
            if (!user.IsAuthor && !user.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized post creation attempt by user {UserId}",
                    user.UserId);

                throw new ForbiddenException(
                    "Only authors or admins can create posts.",
                    "INSUFFICIENT_ROLE");
            }

            var categoryExists = await _categoryRepo
                .GetCategoryQuery(dto.CategoryId)
                .AnyAsync(cancellationToken);

            if (!categoryExists)
                throw new NotFoundException("Category", dto.CategoryId);

            await ValidateTagsExistAsync(dto.TagIds, cancellationToken);

            var post = new Post(
                dto.Title,
                dto.Body,
                user.UserId,
                dto.CategoryId);

            post.SetTags(dto.TagIds ?? []);

            await _postRepo.AddAsync(post, cancellationToken);
            await _postRepo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Post created: {PostId} by user {UserId}",
                post.Id,
                user.UserId);

            return await GetPostByIdAsync(post.Id, user, cancellationToken);
        }

        public async Task<PostReadDTO> UpdatePostAsync(
            Guid postId,
            PostUpdateDTO dto,
            UserContext user,
            CancellationToken cancellationToken)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: false)
                ?? throw new NotFoundException("Post", postId);

            if (!user.IsAdmin && post.AuthorId != user.UserId)
                throw new ForbiddenException("You do not have permission to edit this post.", "NOT_POST_OWNER");

            if (dto.CategoryId.HasValue)
            {
                if (dto.CategoryId.Value == Guid.Empty)
                    throw new BadRequestException("Invalid category.", "INVALID_CATEGORY_ID");
                var catExists = await _categoryRepo
                    .GetCategoryQuery(dto.CategoryId.Value)
                    .AnyAsync(cancellationToken);

                if (!catExists)
                    throw new NotFoundException("Category", dto.CategoryId.Value);
            }

            await ValidateTagsExistAsync(dto.TagIds, cancellationToken);

            post.Update(
                dto.Title ?? post.Title,
                dto.Body ?? post.Body,
                dto.CategoryId ?? post.CategoryId
            );

            if (dto.TagIds != null)
                post.SetTags(dto.TagIds);

            await _postRepo.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Post updated: {PostId} by user {UserId}",
                post.Id,
                user.UserId);

            return await GetPostByIdAsync(post.Id, user, cancellationToken);
        }

        public async Task SoftDeletePostAsync(Guid postId, UserContext user, CancellationToken cancellationToken)
        {
            if (!user.IsAuthor && !user.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized post delete attempt by user {UserId}",
                    user.UserId);

                throw new ForbiddenException(
                    "Only authors or admins can delete posts.",
                    "INSUFFICIENT_ROLE");
            }

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin)
                ?? throw new NotFoundException("Post", postId);

            if (post.DeletedAt != null)
                throw new BadRequestException("Post is already deleted.", "POST_ALREADY_DELETED");

            if (!user.IsAdmin && post.AuthorId != user.UserId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to delete post {PostId} owned by another user",
                    user.UserId,
                    postId);

                throw new ForbiddenException(
                    "You do not have permission to delete this post.",
                    "NOT_POST_OWNER");
            }

            post.SoftDelete();

            await _postRepo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Post soft deleted: {PostId} by user {UserId}",
                post.Id,
                user.UserId);
        }

        public async Task RestorePostAsync(Guid postId, UserContext user, CancellationToken cancellationToken)
        {
            if (!user.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized post restore attempt by user {UserId}",
                    user.UserId);

                throw new ForbiddenException(
                    "Only admins can restore posts.",
                    "ADMIN_ONLY_ACTION");
            }

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true)
                ?? throw new NotFoundException("Post", postId);

            post.Restore();
            await _postRepo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Post restored: {PostId} by admin {UserId}",
                post.Id,
                user.UserId);
        }

        public async Task HardDeletePostAsync(Guid postId, UserContext user, CancellationToken cancellationToken)
        {
            if (!user.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized hard delete attempt by user {UserId}",
                    user.UserId);

                throw new ForbiddenException(
                    "Only admins can hard delete posts.",
                    "ADMIN_ONLY_ACTION");
            }
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true, cancellationToken)
                ?? throw new NotFoundException("Post", postId);

            if (post.DeletedAt == null)
                throw new BadRequestException(
                    "Post must be soft deleted before it can be permanently deleted.",
                    "POST_NOT_DELETED");

            await _postRepo.DeleteAsync(post);
            await _postRepo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Post permanently deleted: {PostId} by admin {UserId}",
                post.Id,
                user.UserId);
        }

        // --- PRIVATE HELPERS ---

        private static async Task<PaginatedResultDTO<PostReadDTO>> ExecutePagedQueryAsync(
            IQueryable<Post> query,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            return await query
                .AsDTO()
                .ToPaginatedResultAsync(
                    page,
                    pageSize,
                    cancellationToken);
        }

        private async Task ValidateTagsExistAsync(IEnumerable<Guid>? tagIds, CancellationToken cancellationToken)
        {
            if (tagIds == null || !tagIds.Any()) return;

            var uniqueTagIds = tagIds.Distinct().ToList();

            var validTagCount = await _tagRepo.GetTagsQuery()
                .Where(t => uniqueTagIds.Contains(t.Id))
                .CountAsync(cancellationToken);

            if (validTagCount != uniqueTagIds.Count)
                throw new NotFoundException("Tag", uniqueTagIds);
        }

        private static IQueryable<Post> ApplyFilters(IQueryable<Post> query, PostQueryParams p)
        {
            if (p.CategoryId is Guid catId)
                query = query.Where(post => post.CategoryId == catId);

            if (p.TagId is Guid tagId)
                query = query.Where(post => post.PostTags.Any(pt => pt.TagId == tagId));

            if (p.AuthorId is Guid authorId)
                query = query.Where(post => post.AuthorId == authorId);

            if (!string.IsNullOrWhiteSpace(p.Keyword))
                query = query.Where(post => post.Title.Contains(p.Keyword));

            return query;
        }

        private static IQueryable<Post> ApplySorting(IQueryable<Post> query)
        {
            return query.OrderByDescending(u => u.CreatedAt);
        }
    }
}