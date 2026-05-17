using BlogFlow.API.Models;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface ITagRepository
    {
        IQueryable<Tag> GetTagsQuery();
        IQueryable<Tag> GetTagQuery(Guid id);

        Task CreateTagAsync(Tag tag, CancellationToken cancellationToken);
        Task<bool> DeleteTagByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> TagExistsAsync(string tagName, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}