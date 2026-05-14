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

        public CommentService(ICommentRepository repo, IPostRepository postRepo)
        {
            _repo = repo;
            _postRepo = postRepo;
        }

        public async Task<CommentReadDTO> CreateAsync(Guid postId, Guid userId, CommentCreateDTO dto)
        {
            var postExists = await _postRepo.GetPostsQuery(includeDeleted: false)
                .AnyAsync(p => p.Id == postId);

            if (!postExists) throw new NotFoundException("Post", postId);

            var comment = new Comment(dto.Body, userId, postId);
            await _repo.AddAsync(comment);
            await _repo.SaveChangesAsync();

            return await _repo.GetQueryable() // Returns IQueryable<Comment>
                .Where(c => c.Id == comment.Id)
                .AsDTO()
                .FirstOrDefaultAsync()
                ?? throw new Exception("Failed to load created comment.");
        }

        //SOFT DELETE
        public async Task DeleteAsync(Guid postId, Guid commentId, Guid userId)
        {
            var comment = await ValidateAsync(commentId, postId, userId);

            comment.SoftDelete();
            await _repo.SaveChangesAsync();
        }

        public async Task<PaginatedResultDTO<CommentReadDTO>> GetByPostAsync(Guid postId, int page, int pageSize)
        {
            var query = _repo.GetCommentsByPostQuery(postId)
                             .OrderByDescending(c => c.CreatedAt);

            var pagedEntities = await query.ToPaginatedResultAsync(page, pageSize);

            return new PaginatedResultDTO<CommentReadDTO>
            {
                TotalCount = pagedEntities.TotalCount,
                Page = pagedEntities.Page,
                PageSize = pagedEntities.PageSize,
                Items = pagedEntities.Items.AsDTO().ToList()
            };
        }

        public async Task<CommentReadDTO> UpdateAsync(Guid postId, Guid commentId, Guid userId, string newBody)
        {
            var comment = await ValidateAsync(commentId, postId, userId);

            comment.UpdateBody(newBody);
            await _repo.SaveChangesAsync();
            return comment.ToDTO();
        }


        public async Task<CommentReadDTO> GetByIdAsync(Guid postId, Guid commentId)
        {
            var comment = await ValidateAsync(commentId, postId);
            return comment.ToDTO();
        }

        private async Task<Comment> ValidateAsync(Guid commentId, Guid postId, Guid? userId = null)
        {
            var comment = await _repo.GetTrackedByIdAsync(commentId)
                ?? throw new NotFoundException("Comment", commentId);

            if (comment.PostId != postId)
                throw new NotFoundException("Comment", commentId);

            if (userId.HasValue && comment.UserId != userId.Value)
                throw new ForbiddenException("You do not have permission to modify this comment.", "NOT_COMMENT_OWNER");

            return comment;
        }
    }
}
