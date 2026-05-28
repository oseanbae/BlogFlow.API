using BlogFlow.API.Data;
using BlogFlow.API.Domain.Entities;
using BlogFlow.API.Helper;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;
        public TagRepository(AppDbContext context) => _context = context;

        public IQueryable<Tag> GetTagsQuery()
            => _context.Tags.AsNoTracking();

        public IQueryable<Tag> GetTagQuery(Guid id)
            => _context.Tags.AsNoTracking().Where(t => t.Id == id);

        public async Task CreateTagAsync(Tag newTag, CancellationToken cancellationToken)
            => await _context.Tags.AddAsync(newTag, cancellationToken);

        public async Task<bool> TagExistsAsync(string tagName, CancellationToken cancellationToken)
        {
            var normalized = SlugHelper.Normalize(tagName);

            return await _context.Tags.AnyAsync(
                t => t.Name == normalized,
                cancellationToken);
        }

        public async Task<bool> DeleteTagByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var tag = await _context.Tags.FindAsync([id], cancellationToken);
            if (tag == null) return false;

            _context.Tags.Remove(tag);
            return true;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
            => _context.SaveChangesAsync(cancellationToken);
    }
}