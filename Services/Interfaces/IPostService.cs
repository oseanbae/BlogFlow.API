using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IPostService
    {
        // Public
        Task<PaginatedPostResultDTO> GetPublishedPostsAsync(
            int page,
            int pageSize,
            Guid? categoryId);

        // Admin or public with filter
        Task<PaginatedPostResultDTO> GetAllPostsAsync(
            int page,
            int pageSize,
            bool isAdmin);

        // Single post
        Task<PostReadDTO> GetPostByIdAsync(
            Guid postId,
            bool isAdmin);

        // Search
        Task<PaginatedPostResultDTO> SearchPostsAsync(
            string keyword,
            int page,
            int pageSize,
            UserRole requesterRole);

        // Filter by tag
        Task<PaginatedPostResultDTO> GetPostsByTagAsync(
            Guid tagId,
            int page,
            int pageSize,
            UserRole requesterRole);

        // Create
        Task<PostReadDTO> CreatePostAsync(
            PostCreateDTO dto,
            Guid authorId);

        // Update
        Task<PostReadDTO> UpdatePostAsync(
            Guid postId,
            PostUpdateDTO dto,
            Guid requesterId,
            bool isAdmin);

        // Soft delete
        Task SoftDeletePostAsync(
            Guid postId,
            Guid requesterId,
            bool isAdmin);

        // Restore — Admin only
        Task RestorePostAsync(
            Guid postId,
            UserRole requesterRole);

        // Hard delete — Admin only
        Task HardDeletePostAsync(
            Guid postId,
            UserRole requesterRole);
    }
}