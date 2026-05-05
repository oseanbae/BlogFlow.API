using BlogFlow.API.Models;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface ITagRepository
    {
        IQueryable<Tag> GetTagsQuery();
        IQueryable<Tag> GetTagQuery(Guid id);
        Task CreateTagAsync(Tag tag);
        Task<bool> DeleteTagByIdAsync(Guid id);
        Task<bool> TagExistsAsync(string tagName);
        Task SaveChangesAsync();
    }
}