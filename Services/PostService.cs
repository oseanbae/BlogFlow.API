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
        public PostService(IPostRepository postRepo, ICategoryRepository categoryRepo, ITagRepository tagRepo)
        {
            _postRepo = postRepo;
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

            return postDto ?? throw new NotFoundException("Post", postId);
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
            if (!user.IsAuthor && !user.IsAdmin)
                throw new ForbiddenException("Only authors or admins can create posts.", "INSUFFICIENT_ROLE");

            var categoryExists = await _categoryRepo.GetCategoryQuery(dto.CategoryId).AnyAsync();
            if (!categoryExists) throw new NotFoundException("Category", dto.CategoryId);

            await ValidateTagsExistAsync(dto.TagIds);

            var post = new Post(dto.Title, dto.Body, user.UserId, dto.CategoryId);

            post.SetTags(dto.TagIds ?? []);

            await _postRepo.AddAsync(post);
            await _postRepo.SaveChangesAsync();

            return await GetPostByIdAsync(post.Id, user);
        }

        public async Task<PostReadDTO> UpdatePostAsync(Guid postId, PostUpdateDTO dto, UserContext user)
        {
            if (!user.IsAuthor && !user.IsAdmin)
                throw new ForbiddenException("Only authors or admins can update posts.", "INSUFFICIENT_ROLE");

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin)
                ?? throw new NotFoundException("Post", postId);

            if (!user.IsAdmin && post.AuthorId != user.UserId)
                throw new ForbiddenException("You do not have permission to update this post.", "NOT_POST_OWNER");

            if (dto.CategoryId.HasValue)
            {
                if (dto.CategoryId.Value == Guid.Empty)
                    throw new BadRequestException("Invalid category.", "INVALID_CATEGORY_ID");

                var catExists = await _categoryRepo
                    .GetCategoryQuery(dto.CategoryId.Value)
                    .AnyAsync();

                if (!catExists) throw new NotFoundException("Category", dto.CategoryId);
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
            if (!user.IsAuthor && !user.IsAdmin)
                throw new ForbiddenException("Only authors or admins can delete posts.", "INSUFFICIENT_ROLE");

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: user.IsAdmin)
                ?? throw new NotFoundException("Post", postId);

            if (!user.IsAdmin && post.AuthorId != user.UserId)
                throw new ForbiddenException("You do not have permission to delete this post.", "NOT_POST_OWNER");

            post.SoftDelete();
            await _postRepo.SaveChangesAsync();
        }

        public async Task RestorePostAsync(Guid postId, UserContext user)
        {
            if (!user.IsAdmin)
                throw new ForbiddenException("Only admins can restore posts.", "ADMIN_ONLY_ACTION");

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true)
                ?? throw new NotFoundException("Post", postId);

            post.Restore();
            await _postRepo.SaveChangesAsync();
        }

        public async Task HardDeletePostAsync(Guid postId, UserContext user)
        {
            if (!user.IsAdmin)
                throw new ForbiddenException("Only admins can hard delete posts.", "ADMIN_ONLY_ACTION");

            var post = await _postRepo.GetTrackedByIdAsync(postId, includeDeleted: true)
                ?? throw new NotFoundException("Post", postId);

            await _postRepo.DeleteAsync(post);
            await _postRepo.SaveChangesAsync();
        }

        // --- PRIVATE HELPER ---

        private static async Task<PaginatedResultDTO<PostReadDTO>> ExecutePagedQueryAsync(
            IQueryable<Post> query, int page, int pageSize)
        {
            var pagedResult = await query
                .OrderByDescending(p => p.CreatedAt)
                .AsDTO()
                .ToPaginatedResultAsync(page, pageSize);

            return new PaginatedResultDTO<PostReadDTO>
            {
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                Items = pagedResult.Items
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

            if (p.AuthorId is Guid authorId)
                query = query.Where(post => post.AuthorId == authorId);

            if (!string.IsNullOrWhiteSpace(p.Keyword))
                query = query.Where(post => post.Title.Contains(p.Keyword));

            return query;
        }
    }
}