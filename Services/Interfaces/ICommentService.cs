using BlogFlow.API.DTOs;
using BlogFlow.API.DTOs.Comment;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentReadDTO> CreateAsync(Guid postId, Guid userId, CommentCreateDTO dto);

        Task<PaginatedResultDTO<CommentReadDTO>> GetByPostAsync(
            Guid postId,
            int page,
            int pageSize);

        Task<CommentReadDTO> UpdateAsync(
            Guid commentId,
            Guid userId,
            string newBody);

        Task DeleteAsync(Guid commentId, Guid userId);
        Task<CommentReadDTO> GetByIdAsync(Guid commentId);
    }
}