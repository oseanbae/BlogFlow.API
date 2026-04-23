using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly ICurrentUserService _currentUser;
        public PostController(IPostService postService, ICurrentUserService currentUserService)
        {
            _postService = postService;
            _currentUser = currentUserService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedPostResultDTO>> GetAllPostsAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var isAdmin = User.IsInRole("Admin");
            var result = await _postService.GetAllPostsAsync(page, pageSize, isAdmin);
            return Ok(result);
        }

        [HttpGet("{postId}")]
        [AllowAnonymous]
        public async Task<ActionResult<PostReadDTO>> GetPostAsync(Guid postId)
        {
            var isAdmin = User.IsInRole("Admin");
            var result = await _postService.GetPostByIdAsync(postId, isAdmin);
            return Ok(result);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Author")]
        public async Task<ActionResult<PostReadDTO>> CreatePostAsync(PostCreateDTO dto)
        {
            var userId = _currentUser.GetRequiredUserId();

            var result = await _postService.CreatePostAsync(dto, userId);

            return Ok(result);
        }

        [HttpPut("{postId}")]
        [Authorize(Roles = "Author, Admin")]
        public async Task<ActionResult<PostReadDTO>> UpdatePostAsync(Guid postId, PostUpdateDTO dto)
        {
            var userId = _currentUser.GetRequiredUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _postService.UpdatePostAsync(postId, dto, userId, isAdmin);
            return Ok(result);
        }

        [HttpDelete("{postId}")]
        [Authorize(Roles = "Author, Admin")]
        public async Task<ActionResult> DeletePostAsync(Guid postId) 
        {
            var userId = _currentUser.GetRequiredUserId();
            var isAdmin = User.IsInRole("Admin");
            await _postService.SoftDeletePostAsync(postId, userId, isAdmin);
            return NoContent();
        }
    }
}
