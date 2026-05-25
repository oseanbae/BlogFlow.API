using BlogFlow.API.DTOs.Comment;
using BlogFlow.API.DTOs.Common;
using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentReadDTO> CreateAsync(Guid postId, UserContext user, CommentCreateDTO dto, CancellationToken cancellationToken);
        Task<PaginatedResultDTO<CommentReadDTO>> GetByPostAsync(Guid postId, CommentQueryParams p, UserContext user, CancellationToken cancellationToken);
        Task<CommentReadDTO> GetByIdAsync(Guid postId, Guid commentId, UserContext user, CancellationToken cancellationToken);
        Task<CommentReadDTO> UpdateAsync(Guid postId, Guid commentId, UserContext user, string newBody, CancellationToken cancellationToken);
        Task DeleteAsync(Guid postId, Guid commentId, UserContext user, CancellationToken cancellationToken);
    }
}