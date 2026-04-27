using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IPostService
    {
        Task<PaginatedPostResultDTO> GetPostsAsync(int page, int pageSize, Guid? categoryId, UserContext user);
        Task<PostReadDTO> GetPostByIdAsync(Guid postId, UserContext user);
        Task<PaginatedPostResultDTO> SearchPostsAsync(string keyword, int page, int pageSize, UserContext user);
        Task<PaginatedPostResultDTO> GetPostsByTagAsync(Guid tagId, int page, int pageSize, UserContext user);
        Task<PostReadDTO> CreatePostAsync(PostCreateDTO dto, UserContext user);
        Task<PostReadDTO> UpdatePostAsync(Guid postId, PostUpdateDTO dto, UserContext user);
        Task SoftDeletePostAsync(Guid postId, UserContext user);
        Task RestorePostAsync(Guid postId, UserContext user);
        Task HardDeletePostAsync(Guid postId, UserContext user);
    }
}