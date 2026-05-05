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
            // 1. Get the base query from the repo (already includes the Soft Delete filter)
            var query = _repo.GetCommentsByPostQuery(postId);

            // 2. Build the pipeline and execute once
            return await query
                .OrderByDescending(c => c.CreatedAt) // Stable sort
                .AsDTO()                             // Project to DTO
                .ToPaginatedResultAsync(page, pageSize); // Executes Count + Fetch
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
