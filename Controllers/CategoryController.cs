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

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryReadDTO>>> GetAllCategoriesAsync(CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetCategoriesAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryReadDTO>> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetCategoryByIdAsync(categoryId, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryReadDTO>> CreateCategoryAsync(CategoryCreateDTO dto, CancellationToken cancellationToken)
        {
            var result = await _categoryService.CreateCategoryAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { categoryId = result.Id }, result);
        }

        [HttpPatch("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RenameCategoryAsync(Guid categoryId, CategoryRenameDTO dto, CancellationToken cancellationToken)
        {
            var result = await _categoryService.RenameCategoryAsync(categoryId, dto.Name, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DeleteCategoryResultDTO>> DeleteCategoryAsync(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            var result = await _categoryService.DeleteCategoryAsync(categoryId, cancellationToken);
            return Ok(result);
        }
    }
}
