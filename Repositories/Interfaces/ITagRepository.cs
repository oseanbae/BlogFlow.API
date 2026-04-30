using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Models;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task CreateTagAsync(Tag tag);
        Task<TagReadDTO?> GetTagByIdAsync(Guid id);
        Task<bool> TagExistsAsync(string tagName);
        Task<bool> DeleteTagByIdAsync(Guid id);
    }
}
