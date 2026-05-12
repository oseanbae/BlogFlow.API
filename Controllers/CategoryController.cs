using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
   
        public CategoryController(ICategoryService service)
        {
            _categoryService = service;
        }

        [HttpGet] // GET   api/v1/categories
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryReadDTO>>> GetAllCategoriesAsync()
        {
            var result = await _categoryService.GetCategoriesAsync();
            return Ok(result);
        }


        [HttpGet("{categoryId}")] // GET   api/v1/categories/{categoryId}
        [AllowAnonymous]
        public async Task<ActionResult<CategoryReadDTO>> GetCategoryByIdAsync(Guid categoryId)
        {
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);
            return Ok(result);
        }

        [HttpPost] // POST  api/v1/categories
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryReadDTO>> CreateCategoryAsync(CategoryCreateDTO dto)
        {
            var result = await _categoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { categoryId = result.Id }, result);
        }

        [HttpPatch("{categoryId}")] // PATCH api/v1/categories/{categoryId}
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RenameCategoryAsync(Guid categoryId, CategoryRenameDTO dto)
        {
            await _categoryService.RenameCategoryAsync(categoryId, dto.Name);
            return NoContent();

        }

    }
}
