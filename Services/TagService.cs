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

        public async Task<IEnumerable<TagReadDTO>> GetAllTagsAsync(CancellationToken cancellationToken)
            => await _repo.GetTagsQuery()
                .AsDTO()
                .ToListAsync(cancellationToken);

        public async Task<TagReadDTO> GetTagByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _repo.GetTagQuery(id)
                .AsDTO()
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Tag", id);

        public async Task<TagReadDTO> CreateTagAsync(TagCreateDTO dto, CancellationToken cancellationToken)
        {
            if (await _repo.TagExistsAsync(dto.Name, cancellationToken))
            {
                _logger.LogWarning(
                    "Tag creation failed - duplicate tag name: {TagName}",
                    dto.Name);

                throw new ConflictException(
                    $"Tag '{dto.Name}' already exists.",
                    "TAG_ALREADY_EXISTS");
            }

            var tag = new Tag(dto.Name);

            await _repo.CreateTagAsync(tag, cancellationToken);

            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Tag created: {TagId} ({TagName})",
                tag.Id,
                tag.Name);

            return await _repo.GetTagQuery(tag.Id)
                .AsDTO()
                .FirstAsync(cancellationToken);
        }

        public async Task DeleteTagAsync(Guid id, CancellationToken cancellationToken)
        {
            if (!await _repo.DeleteTagByIdAsync(id, cancellationToken))
                throw new NotFoundException("Tag", id);

            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Tag deleted: {TagId}",
                id);
        }
    }
}