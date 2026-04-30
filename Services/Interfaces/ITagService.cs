using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ITagService
    {
        Task<TagReadDTO> CreateTagAsync(TagCreateDTO dto);
        Task DeleteTagAsync(Guid id);
        Task<TagReadDTO?> GetTagByIdAsync(Guid id);
    }
}
