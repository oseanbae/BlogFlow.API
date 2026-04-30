using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;

        public TagService(ITagRepository tagRepository)
        {
            _repo = tagRepository;
        }

        //Admin OR Author only 
        public async Task<TagReadDTO> CreateTagAsync(TagCreateDTO dto)
        {
            var tag = new Tag(dto.Name);

            try
            {
                await _repo.CreateTagAsync(tag);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                throw new InvalidOperationException($"Tag '{dto.Name}' already exists.");
            }

            return new TagReadDTO
            {
                Id = tag.Id,
                Name = tag.DisplayName // "ASP.NET" not "asp.net"
            };
        }

        //Admin Only
        public async Task DeleteTagAsync(Guid id)
        {
            var deleted = await _repo.DeleteTagByIdAsync(id);

            if (!deleted)
                throw new KeyNotFoundException($"Tag with ID {id} not found.");
        }

        public async Task<TagReadDTO?> GetTagByIdAsync(Guid id)
        {
            return await _repo.GetTagByIdAsync(id);
        }

        private bool IsUniqueConstraintViolation(DbUpdateException ex)
            => ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true;

    }
}
