using BlogFlow.API.DTOs.Common;
using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{
    [Route("api/v1/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ICurrentUserService _currentUser;

        public PostController(IPostService postService, ICurrentUserService currentUserService)
        {
            _postService = postService;
            _currentUser = currentUserService;
        }

        [HttpGet] // GET api/v1/posts
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedResultDTO<PostReadDTO>>> GetPostsAsync(
            [FromQuery] PostQueryParams p,
            CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.GetPostsAsync(p, user, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{postId}")] // GET api/v1/posts/{postId}
        [AllowAnonymous]
        public async Task<ActionResult<PostReadDTO>> GetPostAsync(
            Guid postId,
            CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.GetPostByIdAsync(postId, user, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{postId}/publish")]
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult<PostReadDTO>> PublishPostAsync(
            Guid postId, CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.PublishPostAsync(postId, user, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{postId}/unpublish")]
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult<PostReadDTO>> UnpublishPostAsync(
            Guid postId, CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.UnpublishPostAsync(postId, user, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{postId}/archive")]
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult<PostReadDTO>> ArchivePostAsync(
            Guid postId, CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.ArchivePostAsync(postId, user, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{postId}/draft")]
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult<PostReadDTO>> MoveToDraftAsync(
            Guid postId, CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.MoveToDraftAsync(postId, user, cancellationToken);
            return Ok(result);
        }


        [HttpPost] // POST api/v1/posts
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult<PostReadDTO>> CreatePostAsync(
            PostCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.CreatePostAsync(dto, user, cancellationToken);
            return CreatedAtAction(nameof(GetPostAsync), new { postId = result.Id }, result);
        }

        [HttpPut("{postId}")] // PUT api/v1/posts/{postId}
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult<PostReadDTO>> UpdatePostAsync(
            Guid postId,
            PostUpdateDTO dto,
            CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.UpdatePostAsync(postId, dto, user, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{postId}")] // DELETE api/v1/posts/{postId}
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult> SoftDeletePostAsync(
            Guid postId,
            CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();
            await _postService.SoftDeletePostAsync(postId, user, cancellationToken);
            return NoContent();
        }

        [HttpPatch("{postId}/restore")] // PATCH api/v1/posts/{postId}/restore
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult> RestorePostAsync(
            Guid postId,
            CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();
            await _postService.RestorePostAsync(postId, user, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{postId}/hard")] // DELETE api/v1/posts/{postId}/hard
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> HardDeletePostAsync(
            Guid postId,
            CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();
            await _postService.HardDeletePostAsync(postId, user, cancellationToken);
            return NoContent();
        }
    }
}