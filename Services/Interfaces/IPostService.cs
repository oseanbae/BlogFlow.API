using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;

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
            bool includeDeleted
        );

        // Single post with access control
        Task<PostReadDTO> GetPostByIdAsync(
            Guid postId,
            Guid requesterId,
            UserRole requesterRole
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
            UserRole requesterRole
        );

        // Soft delete
        Task SoftDeletePostAsync(
            Guid postId,
            Guid requesterId,
            UserRole requesterRole
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