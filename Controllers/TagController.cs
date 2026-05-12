using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _service;

        public TagController(ITagService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TagReadDTO>>> GetAllTagsAsync()
        {
            var tags = await _service.GetAllTagsAsync();
            return Ok(tags);
        }

        [HttpGet("{tagId}")]
        [AllowAnonymous]
        public async Task<ActionResult<TagReadDTO>> GetTagByIdAsync(Guid tagId)
        {
            var result = await _service.GetTagByIdAsync(tagId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult<TagReadDTO>> CreateTagAsync(TagCreateDTO dto)
        {
            var result = await _service.CreateTagAsync(dto);
            return CreatedAtAction
            (
                nameof(GetTagByIdAsync),
                new { tagId = result.Id },
                result
            );
        }

        [HttpDelete("{tagId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTagAsync(Guid tagId)
        {
            await _service.DeleteTagAsync(tagId);
            return NoContent();
        }

    }
}
