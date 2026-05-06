using BlogFlow.API.Models;
using BlogFlow.API.DTOs;
using BlogFlow.API.DTOs.Comment;
using BlogFlow.API.Queries;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;

        public CommentService(ICommentRepository repo) => _repo = repo;

        public async Task<CommentReadDTO> CreateAsync(Guid postId, Guid userId, CommentCreateDTO dto)
        {
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
        public async Task DeleteAsync(Guid commentId, Guid userId)
        {

            var comment = await _repo.GetTrackedByIdAsync(commentId)
                ?? throw new KeyNotFoundException("Comment not found.");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own comments.");

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

        public async Task<CommentReadDTO> UpdateAsync(Guid commentId, Guid userId, string newBody)
        {
            var comment = await _repo.GetTrackedByIdAsync(commentId)
                ?? throw new KeyNotFoundException("Comment not found.");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("Not authorized.");

            comment.UpdateBody(newBody);
            await _repo.SaveChangesAsync();

            return comment.ToDTO();
        }

        public async Task<CommentReadDTO> GetByIdAsync(Guid commentId)
        {
            var comment = await _repo.GetTrackedByIdAsync(commentId)
                ?? throw new KeyNotFoundException("Comment not found.");

            return comment.ToDTO();
        } 
    }
}
