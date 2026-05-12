using BlogFlow.API.DTOs.Tag;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagReadDTO>> GetAllTagsAsync();
        Task<TagReadDTO> GetTagByIdAsync(Guid id);
        Task<TagReadDTO> CreateTagAsync(TagCreateDTO dto);
        Task DeleteTagAsync(Guid id);
    }
}