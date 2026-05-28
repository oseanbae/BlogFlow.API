using BlogFlow.API.Domain.Entities;
using BlogFlow.API.Domain.QueryParams;
using BlogFlow.API.DTOs.Common;
using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Exceptions;
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
            var query = _postRepo.GetPostsQuery(includeDeleted: user.IsAdmin)
                .Where(p => p.Id == postId);

            query = ApplyReadVisibility(query, user);

            var postDto = await query
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

            query = ApplyReadVisibility(query, user);
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

            ValidateOwnership(post, user);

            if (post.State != PostState.Draft)
                throw new BadRequestException(
                    "Only draft posts can be edited. Unpublish or move to draft first.",
                    "POST_NOT_EDITABLE");

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

        public async Task<PostReadDTO> PublishPostAsync(
            Guid postId,
            UserContext user,
            CancellationToken cancellationToken)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin, cancellationToken)
                ?? throw new NotFoundException("Post", postId);

            ValidateOwnership(post, user);

            post.Publish();

            await _postRepo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Post {PostId} published successfully by user {UserId}", postId, user.UserId);
            return await GetPostByIdAsync(post.Id, user, cancellationToken);
        }

        public async Task<PostReadDTO> ArchivePostAsync(
            Guid postId,
            UserContext user,
            CancellationToken cancellationToken)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin, cancellationToken)
                ?? throw new NotFoundException("Post", postId);

            ValidateOwnership(post, user);

            post.Archive();

            await _postRepo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Post {PostId} archived successfully by user {UserId}", postId, user.UserId);
            return await GetPostByIdAsync(post.Id, user, cancellationToken);
        }
        public async Task<PostReadDTO> UnpublishPostAsync(
            Guid postId,
            UserContext user,
            CancellationToken cancellationToken)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin, cancellationToken)
                ?? throw new NotFoundException("Post", postId);

            ValidateOwnership(post, user);

            post.Unpublish();

            await _postRepo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Post {PostId} unpublished successfully by user {UserId}",
                postId,
                user.UserId);

            return await GetPostByIdAsync(post.Id, user, cancellationToken);
        }

        public async Task<PostReadDTO> MoveToDraftAsync(
            Guid postId,
            UserContext user,
            CancellationToken cancellationToken)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin, cancellationToken)
                ?? throw new NotFoundException("Post", postId);

            ValidateOwnership(post, user);

            post.MoveToDraft();

            await _postRepo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Post {PostId} reverted to draft successfully by user {UserId}", postId, user.UserId);
            return await GetPostByIdAsync(post.Id, user, cancellationToken);
        }

        public async Task SoftDeletePostAsync(Guid postId, UserContext user, CancellationToken cancellationToken)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin)
                ?? throw new NotFoundException("Post", postId);

            if (post.DeletedAt != null)
                throw new BadRequestException("Post is already deleted.", "POST_ALREADY_DELETED");

            ValidateOwnership(post, user);

            post.SoftDelete();

            await _postRepo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Post soft deleted: {PostId} by user {UserId}",
                post.Id,
                user.UserId);
        }

        public async Task RestorePostAsync(Guid postId, UserContext user, CancellationToken cancellationToken)
        {
            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true, cancellationToken)
                ?? throw new NotFoundException("Post", postId);

            ValidateOwnership(post, user);

            post.Restore();
            await _postRepo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Post {PostId} restored by user {UserId} ({Role})",
                post.Id,
                user.UserId,
                user.Role);
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
            if (p.State.HasValue)
                query = query.Where(post => post.State == p.State.Value);

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

        private static void ValidateOwnership(Post post, UserContext user)
        {
            if (!user.IsAuthenticated)
                throw new UnauthorizedException("User context is unauthenticated.", "UNAUTHENTICATED");

            if (user.Role == UserRole.Reader)
                throw new ForbiddenException("Readers are forbidden from mutating post data.", "INSUFFICIENT_ROLE");

            if (user.IsAdmin)
                return;

            if (user.IsAuthor && post.AuthorId != user.UserId)
                throw new ForbiddenException("You do not have permission to modify this post.", "NOT_POST_OWNER");
        }

        private static IQueryable<Post> ApplyReadVisibility(IQueryable<Post> query, UserContext user)
        {
            if (user.IsAdmin)
                return query; // admin sees everything

            if (user.IsAuthor)
                // authors see published posts + their own drafts/archived
                return query.Where(p =>
                    p.State == PostState.Published ||
                    p.AuthorId == user.UserId);

            // anonymous and readers only see published
            return query.Where(p => p.State == PostState.Published);
        }
    }
}