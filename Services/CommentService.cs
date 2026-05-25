using BlogFlow.API.DTOs.Comment;
using BlogFlow.API.DTOs.Common;
using BlogFlow.API.Exceptions;
using BlogFlow.API.Models;
using BlogFlow.API.QueryExtensions;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;
        private readonly IPostRepository _postRepo;
        private readonly ILogger<CommentService> _logger;

        public CommentService(
            ICommentRepository repo,
            IPostRepository postRepo,
            ILogger<CommentService> logger)
        {
            _repo = repo;
            _postRepo = postRepo;
            _logger = logger;
        }

        public async Task<CommentReadDTO> CreateAsync(
            Guid postId,
            UserContext user,
            CommentCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var postQuery = _postRepo.GetPostsQuery(includeDeleted: false)
                .Where(p => p.Id == postId);

            postQuery = ApplyPostVisibility(postQuery, user);

            var post = await postQuery.FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Post", postId);

            if (post.State != PostState.Published)
                throw new BadRequestException(
                    "Comments can only be added to published posts.",
                    "POST_NOT_PUBLISHED");

            var comment = new Comment(dto.Body, user.UserId, postId);

            await _repo.AddAsync(comment, cancellationToken);
            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Comment created: {CommentId} by user {UserId} on post {PostId}",
                comment.Id,
                user.UserId,
                postId);

            return await _repo.GetQueryable()
                .Where(c => c.Id == comment.Id)
                .AsDTO()
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException("Failed to load created comment.");
        }

        public async Task DeleteAsync(
            Guid postId,
            Guid commentId,
            UserContext user,
            CancellationToken cancellationToken)
        {
            var comment = await ValidateAsync(commentId, postId, user, cancellationToken);

            comment.SoftDelete();

            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Comment soft deleted: {CommentId} by user {UserId}",
                commentId,
                user.UserId);
        }

        public async Task<PaginatedResultDTO<CommentReadDTO>> GetByPostAsync(
            Guid postId,
            CommentQueryParams p,
            UserContext user,
            CancellationToken cancellationToken)
        {
            var postQuery = _postRepo.GetPostsQuery(includeDeleted: false)
                .Where(post => post.Id == postId);

            postQuery = ApplyPostVisibility(postQuery, user);

            if (!await postQuery.AnyAsync(cancellationToken))
                throw new NotFoundException("Post", postId);

            var query = _repo.GetCommentsByPostQuery(postId);

            query = ApplyFilters(query, p);
            query = ApplySorting(query);

            return await query
                .AsDTO()
                .ToPaginatedResultAsync(p.Page, p.PageSize, cancellationToken);
        }

        public async Task<CommentReadDTO> UpdateAsync(
            Guid postId,
            Guid commentId,
            UserContext user,
            string newBody,
            CancellationToken cancellationToken)
        {
            var comment = await ValidateAsync(commentId, postId, user, cancellationToken);

            comment.UpdateBody(newBody);

            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Comment updated: {CommentId} by user {UserId}",
                commentId,
                user.UserId);

            return comment.ToDTO();
        }

        public async Task<CommentReadDTO> GetByIdAsync(
            Guid postId,
            Guid commentId,
            UserContext user,
            CancellationToken cancellationToken)
        {
            var postQuery = _postRepo.GetPostsQuery(includeDeleted: false)
                .Where(p => p.Id == postId);

            postQuery = ApplyPostVisibility(postQuery, user);

            if (!await postQuery.AnyAsync(cancellationToken))
                throw new NotFoundException("Post", postId);

            return await _repo.GetQueryable()
                .Where(c => c.Id == commentId && c.PostId == postId)
                .AsDTO()
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Comment", commentId);
        }

        private async Task<Comment> ValidateAsync(
            Guid commentId,
            Guid postId,
            UserContext user,
            CancellationToken cancellationToken)
        {
            var comment = await _repo.GetTrackedByIdAsync(commentId, cancellationToken)
                ?? throw new NotFoundException("Comment", commentId);

            if (comment.PostId != postId)
                throw new NotFoundException("Comment", commentId);

            if (!user.IsAdmin)
            {
                var isCommentOwner = comment.UserId == user.UserId;

                var isPostOwner = user.IsAuthor && await _postRepo
                    .GetPostsQuery(includeDeleted: false)
                    .AnyAsync(p => p.Id == postId && p.AuthorId == user.UserId, cancellationToken);

                if (!isCommentOwner && !isPostOwner)
                {
                    _logger.LogWarning(
                        "Unauthorized comment modification attempt. User {UserId} tried to modify comment {CommentId}",
                        user.UserId,
                        commentId);

                    throw new ForbiddenException(
                        "You do not have permission to modify this comment.",
                        "NOT_COMMENT_OWNER");
                }
            }

            return comment;
        }

        private static IQueryable<Comment> ApplyFilters(IQueryable<Comment> query, CommentQueryParams p)
        {
            if (p.AuthorId.HasValue)
                query = query.Where(c => c.UserId == p.AuthorId.Value);

            if (p.CreatedAfter.HasValue)
                query = query.Where(c => c.CreatedAt >= p.CreatedAfter.Value);

            if (p.CreatedBefore.HasValue)
                query = query.Where(c => c.CreatedAt <= p.CreatedBefore.Value);

            return query;
        }

        private static IQueryable<Comment> ApplySorting(IQueryable<Comment> query)
        {
            // TODO: extend to support dynamic sorting
            return query.OrderByDescending(c => c.CreatedAt);
        }

        private static IQueryable<Post> ApplyPostVisibility(
            IQueryable<Post> query,
            UserContext user)
        {
            if (user.IsAdmin)
                return query;

            if (user.IsAuthor)
                return query.Where(p =>
                    p.State == PostState.Published ||
                    p.AuthorId == user.UserId);

            return query.Where(p => p.State == PostState.Published);
        }
    }
}
