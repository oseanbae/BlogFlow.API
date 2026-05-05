using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using BlogFlow.API.Queries;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;
        public TagService(ITagRepository tagRepository) => _repo = tagRepository;

        public async Task<IEnumerable<TagReadDTO>> GetAllTagsAsync()
            => await _repo.GetTagsQuery().AsDTO().ToListAsync();

        public async Task<TagReadDTO?> GetTagByIdAsync(Guid id)
            => await _repo.GetTagQuery(id).AsDTO().FirstOrDefaultAsync();

        public async Task<TagReadDTO> CreateTagAsync(TagCreateDTO dto)
        {
            if (await _repo.TagExistsAsync(dto.Name))
                throw new InvalidOperationException("Tag already exists.");

            var tag = new Tag(dto.Name);
            await _repo.CreateTagAsync(tag);
            await _repo.SaveChangesAsync();

            return await _repo.GetTagQuery(tag.Id).AsDTO().FirstAsync();
        }

        public async Task DeleteTagAsync(Guid id)
        {
            if (!await _repo.DeleteTagByIdAsync(id))
                throw new KeyNotFoundException();

            await _repo.SaveChangesAsync();
        }
    }
}