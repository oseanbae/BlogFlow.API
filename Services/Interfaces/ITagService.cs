using BlogFlow.API.DTOs.Tag;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagReadDTO>> GetAllTagsAsync(CancellationToken cancellationToken);
        Task<TagReadDTO> GetTagByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<TagReadDTO> CreateTagAsync(TagCreateDTO dto, CancellationToken cancellationToken);
        Task DeleteTagAsync(Guid id, CancellationToken cancellationToken);
    }
}