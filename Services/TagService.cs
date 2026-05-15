using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Exceptions;
using BlogFlow.API.Models;
using BlogFlow.API.QueryExtensions;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;
        private readonly ILogger<TagService> _logger;

        public TagService(
            ITagRepository tagRepository,
            ILogger<TagService> logger)
        {
            _repo = tagRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TagReadDTO>> GetAllTagsAsync()
            => await _repo.GetTagsQuery()
                .AsDTO()
                .ToListAsync();

        public async Task<TagReadDTO> GetTagByIdAsync(Guid id)
        {
            return await _repo.GetTagQuery(id)
                .AsDTO()
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Tag", id);
        }

        public async Task<TagReadDTO> CreateTagAsync(TagCreateDTO dto)
        {
            if (await _repo.TagExistsAsync(dto.Name))
            {
                _logger.LogWarning(
                    "Tag creation failed - duplicate tag name: {TagName}",
                    dto.Name);

                throw new ConflictException(
                    $"Tag '{dto.Name}' already exists.",
                    "TAG_ALREADY_EXISTS");
            }

            var tag = new Tag(dto.Name);

            await _repo.CreateTagAsync(tag);
            await _repo.SaveChangesAsync();

            _logger.LogInformation(
                "Tag created: {TagId} ({TagName})",
                tag.Id,
                tag.Name);

            return await _repo.GetTagQuery(tag.Id)
                .AsDTO()
                .FirstAsync();
        }

        public async Task DeleteTagAsync(Guid id)
        {
            if (!await _repo.DeleteTagByIdAsync(id))
                throw new NotFoundException("Tag", id);

            await _repo.SaveChangesAsync();

            _logger.LogInformation(
                "Tag deleted: {TagId}",
                id);
        }
    }
}