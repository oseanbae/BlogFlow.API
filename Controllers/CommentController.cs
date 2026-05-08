using BlogFlow.API.DTOs.Comment;
using BlogFlow.API.DTOs.Common;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{
    [Route("api/v1/comments")]
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

        // CREATE comment for a post
        // POST: /api/v1/comments/post/{postId}
        [HttpPost("post/{postId}")]
        [Authorize]
        public async Task<ActionResult<CommentReadDTO>> CreateCommentAsync(
            Guid postId,
            [FromBody] CommentCreateDTO dto)
        {
            var user = _currentUser.GetCurrentUser();

            var result = await _service.CreateAsync(postId, user.UserId, dto);

            return CreatedAtAction(
                nameof(GetCommentByIdAsync),
                new { commentId = result.Id },
                result);
        }

        // GET comments for a post
        // GET: /api/v1/comments/post/{postId}
        [HttpGet("post/{postId}")]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedResultDTO<CommentReadDTO>>> GetCommentsByPostAsync(
            Guid postId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetByPostAsync(postId, page, pageSize);
            return Ok(result);
        }

        // GET single comment
        // GET: /api/v1/comments/{commentId}
        [HttpGet("{commentId}")]
        [AllowAnonymous]
        public async Task<ActionResult<CommentReadDTO>> GetCommentByIdAsync(Guid commentId)
        {
            var result = await _service.GetByIdAsync(commentId);
            return Ok(result);
        }

        // UPDATE comment
        // PATCH: /api/v1/comments/{commentId}
        [HttpPatch("{commentId}")]
        [Authorize]
        public async Task<ActionResult<CommentReadDTO>> UpdateCommentAsync(
            Guid commentId,
            [FromBody] CommentUpdateDTO dto)
        {
            var userId = _currentUser.GetCurrentUser().UserId;

            var result = await _service.UpdateAsync(commentId, userId, dto.Body);

            return Ok(result);
        }

        // DELETE comment (soft delete)
        // DELETE: /api/v1/comments/{commentId}
        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<ActionResult> DeleteCommentAsync(Guid commentId)
        {
            var user = _currentUser.GetCurrentUser();

            await _service.DeleteAsync(commentId, user.UserId);

            return NoContent();
        }
    }
}