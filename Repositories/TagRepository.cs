using BlogFlow.API.Data;
using BlogFlow.API.Models;
using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using BlogFlow.API.Queries;

namespace BlogFlow.API.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateTagAsync(Tag newTag)
        {
            await _context.Tags.AddAsync(newTag);
            await _context.SaveChangesAsync();
        }

        public async Task<TagReadDTO?> GetTagByIdAsync(Guid id)
        {
            return await _context.Tags
                .AsNoTracking()
                .Where(c => c.Id == id) 
                .AsDTO()  
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteTagByIdAsync(Guid id)
        {
            var tag = new Tag { Id = id };

            _context.Tags.Attach(tag);
            _context.Tags.Remove(tag);

            try
            {
                var affected = await _context.SaveChangesAsync();
                return affected > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false; // tag didn't exist
            }
        }

        public async Task<bool> TagExistsAsync(string tagName)
        {
            return await _context.Tags.AnyAsync(t => t.Name == tagName);
        }
    }
}
