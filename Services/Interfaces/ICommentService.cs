using BlogFlow.API.DTOs.Comment;
using BlogFlow.API.DTOs.Common;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentReadDTO> CreateAsync(Guid postId, Guid userId, CommentCreateDTO dto, CancellationToken cancellationToken);
        Task<CommentReadDTO> GetByIdAsync(Guid postId, Guid commentId, CancellationToken cancellationToken);
        Task<PaginatedResultDTO<CommentReadDTO>> GetByPostAsync(Guid postId, int page, int pageSize, CancellationToken cancellationToken);
        Task<CommentReadDTO> UpdateAsync(Guid postId, Guid commentId, Guid userId, string newBody, CancellationToken cancellationToken);
        Task DeleteAsync(Guid postId, Guid commentId, Guid userId, CancellationToken cancellationToken);
    }
}