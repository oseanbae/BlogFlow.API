using BlogFlow.API.DTOs.Comment;
using BlogFlow.API.DTOs.Common;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ICommentService
    {
            Task<CommentReadDTO> CreateAsync(Guid postId, Guid userId, CommentCreateDTO dto);
            Task<CommentReadDTO> GetByIdAsync(Guid postId, Guid commentId);
            Task<PaginatedResultDTO<CommentReadDTO>> GetByPostAsync(Guid postId, int page, int pageSize);
            Task<CommentReadDTO> UpdateAsync(Guid postId, Guid commentId, Guid userId, string newBody);
            Task DeleteAsync(Guid postId, Guid commentId, Guid userId);
    }
}