using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;
using System.Security.Claims;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IPostService
    {
        // Public (published posts only)
        Task<PaginatedPostResultDTO> GetPublishedPostsAsync(
            int page,
            int pageSize,
            Guid? categoryId
        );

        // Admin view (can include deleted)
        Task<PaginatedPostResultDTO> GetAllPostsAsync(
            int page,
            int pageSize,
            bool isAdmin
        );

        // Single post with access control
        Task<PostReadDTO> GetPostByIdAsync(
            Guid postId,
            bool isAdmin
        );

        // Search (respect visibility rules)
        Task<PaginatedPostResultDTO> SearchPostsAsync(
            string keyword,
            int page,
            int pageSize,
            Guid requesterId,
            UserRole requesterRole
        );

        // Filter by tag
        Task<PaginatedPostResultDTO> GetPostsByTagAsync(
            Guid tagId,
            int page,
            int pageSize,
            Guid requesterId,
            UserRole requesterRole
        );

        // Create
        Task<PostReadDTO> CreatePostAsync(
            PostCreateDTO dto,
            Guid authorId
        );

        // Update
        Task<PostReadDTO> UpdatePostAsync(
            Guid postId,
            PostUpdateDTO dto,
            Guid requesterId,
            bool idAdmin
        );

        // Soft delete
        Task SoftDeletePostAsync(
            Guid postId,
            Guid requesterId,
            bool isAdmin
        );

        // Restore (Admin only)
        Task RestorePostAsync(
            Guid postId,
            Guid requesterId,
            UserRole requesterRole
        );

        // Hard delete (Admin only)
        Task HardDeletePostAsync(
            Guid postId,
            Guid requesterId,
            UserRole requesterRole
        );
    }
}