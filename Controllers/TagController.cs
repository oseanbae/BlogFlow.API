using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{
    [Route("api/v1/tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _service;

        public TagController(ITagService service)
        {
            _service = service;
        }

        [HttpGet] // GET    api/v1/tags
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TagReadDTO>>> GetAllTagsAsync(CancellationToken cancellationToken)
        {
            var tags = await _service.GetAllTagsAsync(cancellationToken);
            return Ok(tags);
        }

        [HttpGet("{tagId}")] // GET    api/v1/tags/{tagId}
        [AllowAnonymous]
        public async Task<ActionResult<TagReadDTO>> GetTagByIdAsync(Guid tagId, CancellationToken cancellationToken)
        {
            var result = await _service.GetTagByIdAsync(tagId, cancellationToken);
            return Ok(result);
        }

        [HttpPost] // POST   api/v1/tags
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult<TagReadDTO>> CreateTagAsync(TagCreateDTO dto, CancellationToken cancellationToken)
        {
            var result = await _service.CreateTagAsync(dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetTagByIdAsync),
                new { tagId = result.Id },
                result
            );
        }

        [HttpDelete("{tagId}")] // DELETE api/v1/tags/{tagId}
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTagAsync(Guid tagId, CancellationToken cancellationToken)
        {
            await _service.DeleteTagAsync(tagId, cancellationToken);
            return NoContent();
        }
    }
}