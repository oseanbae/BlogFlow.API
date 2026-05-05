using BlogFlow.API.Data;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;
        public TagRepository(AppDbContext context) => _context = context;

        public IQueryable<Tag> GetTagsQuery() => _context.Tags.AsNoTracking();

        public IQueryable<Tag> GetTagQuery(Guid id)
            => _context.Tags.AsNoTracking().Where(t => t.Id == id);

        public async Task CreateTagAsync(Tag newTag) => await _context.Tags.AddAsync(newTag);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<bool> TagExistsAsync(string tagName)
        {
            var normalized = Tag.Normalize(tagName);
            return await _context.Tags.AnyAsync(t => t.Name == normalized);
        }

        public async Task<bool> DeleteTagByIdAsync(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null) return false;

            _context.Tags.Remove(tag);
            return true;
        }
    }
}