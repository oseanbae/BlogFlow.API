using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{
    [Route("api/v1/[controller]")]
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

        // GET api/v1/post
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedPostResultDTO>> GetPostsAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? categoryId = null)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.GetPostsAsync(page, pageSize, categoryId, user);
            return Ok(result);
        }

        // GET api/v1/post/{postId}
        [HttpGet("{postId}")]
        [AllowAnonymous]
        public async Task<ActionResult<PostReadDTO>> GetPostAsync(Guid postId)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.GetPostByIdAsync(postId, user);
            return Ok(result);
        }

        // GET api/v1/post/search?keyword=xxx
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedPostResultDTO>> SearchPostsAsync(
            [FromQuery] string keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.SearchPostsAsync(keyword, page, pageSize, user);
            return Ok(result);
        }

        // GET api/v1/post/tag/{tagId}
        [HttpGet("tag/{tagId}")]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedPostResultDTO>> GetPostsByTagAsync(
            Guid tagId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.GetPostsByTagAsync(tagId, page, pageSize, user);
            return Ok(result);
        }

        // POST api/v1/post
        [HttpPost]
        [Authorize(Roles = "Author")]
        public async Task<ActionResult<PostReadDTO>> CreatePostAsync(PostCreateDTO dto)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.CreatePostAsync(dto, user);
            return CreatedAtAction(nameof(GetPostAsync), new { postId = result.Id }, result);
        }

        // PUT api/v1/post/{postId}
        [HttpPut("{postId}")]
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult<PostReadDTO>> UpdatePostAsync(Guid postId, PostUpdateDTO dto)
        {
            var user = _currentUser.GetCurrentUser();
            var result = await _postService.UpdatePostAsync(postId, dto, user);
            return Ok(result);
        }

        // DELETE api/v1/post/{postId}
        [HttpDelete("{postId}")]
        [Authorize(Roles = "Author,Admin")]
        public async Task<ActionResult> SoftDeletePostAsync(Guid postId)
        {
            var user = _currentUser.GetCurrentUser();
            await _postService.SoftDeletePostAsync(postId, user);
            return NoContent();
        }

        // PATCH api/v1/post/{postId}/restore
        [HttpPatch("{postId}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RestorePostAsync(Guid postId)
        {
            var user = _currentUser.GetCurrentUser();
            await _postService.RestorePostAsync(postId, user);
            return NoContent();
        }

        // DELETE api/v1/post/{postId}/hard
        [HttpDelete("{postId}/hard")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> HardDeletePostAsync(Guid postId)
        {
            var user = _currentUser.GetCurrentUser();
            await _postService.HardDeletePostAsync(postId, user);
            return NoContent();
        }
    }
}