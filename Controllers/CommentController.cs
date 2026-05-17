using BlogFlow.API.DTOs.Comment;
using BlogFlow.API.DTOs.Common;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{
    [Route("api/v1/posts")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _service;
        private readonly ICurrentUserService _currentUser;

        public CommentsController(
            ICommentService commentService,
            ICurrentUserService currentUserService)
        {
            _service = commentService;
            _currentUser = currentUserService;
        }

        [HttpPost("{postId}/comments")] // POST   api/v1/posts/{postId}/comments
        [Authorize]
        public async Task<ActionResult<CommentReadDTO>> CreateCommentAsync(
            Guid postId,
            [FromBody] CommentCreateDTO dto,
            CancellationToken cancellationToken)
        {
            var user = _currentUser.GetCurrentUser();

            var result = await _service.CreateAsync(postId, user.UserId, dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetCommentByIdAsync),
                new { postId, commentId = result.Id },
                result);
        }

        [HttpGet("{postId}/comments")] // GET    api/v1/posts/{postId}/comments
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedResultDTO<CommentReadDTO>>> GetCommentsByPostAsync(
            Guid postId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetByPostAsync(postId, page, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{postId}/comments/{commentId}")] // GET api/v1/posts/{postId}/comments/{commentId}
        [AllowAnonymous]
        public async Task<ActionResult<CommentReadDTO>> GetCommentByIdAsync(
            Guid postId,
            Guid commentId,
            CancellationToken cancellationToken)
            => Ok(await _service.GetByIdAsync(postId, commentId, cancellationToken));

        [HttpPatch("{postId}/comments/{commentId}")] // PATCH
        [Authorize]
        public async Task<ActionResult<CommentReadDTO>> UpdateCommentAsync(
            Guid postId,
            Guid commentId,
            [FromBody] CommentUpdateDTO dto,
            CancellationToken cancellationToken)
            => Ok(await _service.UpdateAsync(
                postId,
                commentId,
                _currentUser.GetRequiredUserId(),
                dto.Body,
                cancellationToken));

        [HttpDelete("{postId}/comments/{commentId}")] // DELETE
        [Authorize]
        public async Task<ActionResult> DeleteComment(
            Guid postId,
            Guid commentId,
            CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(postId, commentId, _currentUser.GetRequiredUserId(), cancellationToken);
            return NoContent();
        }
    }
}