using BlogFlow.API.DTOs.Post;
using BlogFlow.API.Models;
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

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedPostResultDTO>> GetAllPostsAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _postService.GetAllPostsAsync(page, pageSize, User);
            return Ok(result);
        }

    }
}
