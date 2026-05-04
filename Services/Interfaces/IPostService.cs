using BlogFlow.API.DTOs;
using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IPostService
    {
        // Read Operations
        Task<PaginatedResultDTO<PostReadDTO>> GetPostsAsync(int page, int pageSize, Guid? categoryId, UserContext user);
        Task<PostReadDTO> GetPostByIdAsync(Guid postId, UserContext user);
        Task<PaginatedResultDTO<PostReadDTO>> SearchPostsAsync(string keyword, int page, int pageSize, UserContext user);
        Task<PaginatedResultDTO<PostReadDTO>> GetPostsByTagAsync(Guid tagId, int page, int pageSize, UserContext user);

        // Write Operations
        Task<PostReadDTO> CreatePostAsync(PostCreateDTO dto, UserContext user);
        Task<PostReadDTO> UpdatePostAsync(Guid postId, PostUpdateDTO dto, UserContext user);

        // Deletion/Recovery Operations
        Task SoftDeletePostAsync(Guid postId, UserContext user);
        Task RestorePostAsync(Guid postId, UserContext user);
        Task HardDeletePostAsync(Guid postId, UserContext user);
    }
}