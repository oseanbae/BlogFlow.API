using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;

public interface IPostRepository
{
    // Read — returns DTOs directly
    Task<PostReadDTO?> GetByIdAsync(Guid postId, bool includeDeleted = false);

    Task<(IEnumerable<PostReadDTO> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Guid? authorId,
        Guid? categoryId,
        Guid? tagId,
        bool includeDeleted = false);

    Task<(IEnumerable<PostReadDTO> Items, int TotalCount)> SearchAsync(
        string keyword,
        int page,
        int pageSize,
        bool includeDeleted = false);

    // Write — returns tracked entity for domain method calls
    Task<Post?> GetTrackedByIdAsync(Guid postId, bool includeDeleted = false);

    Task AddAsync(Post post);
    Task DeleteAsync(Post post);
    Task SaveChangesAsync();
}