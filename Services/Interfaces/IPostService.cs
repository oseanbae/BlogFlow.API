using BlogFlow.API.DTOs.Common;
using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IPostService
    {
        // Read Operations
        Task<PostReadDTO> GetPostByIdAsync(Guid postId, UserContext user, CancellationToken cancellationToken);
        Task<PaginatedResultDTO<PostReadDTO>> GetPostsAsync(PostQueryParams p, UserContext user, CancellationToken cancellationToken);

        // Write Operations
        Task<PostReadDTO> CreatePostAsync(PostCreateDTO dto, UserContext user, CancellationToken cancellationToken);
        Task<PostReadDTO> UpdatePostAsync(Guid postId, PostUpdateDTO dto, UserContext user, CancellationToken cancellationToken);

        Task<PostReadDTO> PublishPostAsync(Guid postId, UserContext user, CancellationToken cancellationToken);
        Task<PostReadDTO> ArchivePostAsync(Guid postId, UserContext user, CancellationToken cancellationToken);
        Task<PostReadDTO> UnpublishPostAsync(Guid postId, UserContext user, CancellationToken cancellationToken);
        Task<PostReadDTO> MoveToDraftAsync(Guid postId, UserContext user, CancellationToken cancellationToken);

        // Deletion/Recovery Operations
        Task SoftDeletePostAsync(Guid postId, UserContext user, CancellationToken cancellationToken);
        Task RestorePostAsync(Guid postId, UserContext user, CancellationToken cancellationToken);
        Task HardDeletePostAsync(Guid postId, UserContext user, CancellationToken cancellationToken);
    }
}