using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
   
        public CategoryController(ICategoryService service)
        {
            _categoryService = service;
        }

        // GET /api/v1/category
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryReadDTO>>> GetAllCategoriesAsync()
        {
            var result = await _categoryService.GetCategoriesAsync();
            return Ok(result);
        }

        // Get api/v1/category/{categoryId}
        [HttpGet("{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryReadDTO>> GetCategoryByIdAsync(Guid categoryId)
        {
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);
            return Ok(result);
        }

        // Post /api/v1/category
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryReadDTO>> CreateCategoryAsync(CategoryCreateDTO dto)
        {
            var result = await _categoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategoryByIdAsync),new { categoryId = result.Id }, result);
        }

        // Patch api/v1/categories/{categoryId}
        [HttpPatch("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RenameCategoryAsync(Guid categoryId, CategoryRenameDTO dto)
        {
            await _categoryService.RenameCategoryAsync(categoryId, dto.Name);
            return NoContent();

        }

    }
}
