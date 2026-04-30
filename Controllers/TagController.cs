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
        private readonly ICurrentUserService _currentUser;

        public TagController(ITagService service, ICurrentUserService currentUser)
        {
            _service = service;
            _currentUser = currentUser;
        }

        [HttpGet("{tagId}")]
        [AllowAnonymous]
        public async Task<ActionResult<TagReadDTO>> GetTagByIdAsync(Guid tagId)
        {
            var result = await _service.GetTagByIdAsync(tagId);
            return result == null ? NotFound() : Ok(result);
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
