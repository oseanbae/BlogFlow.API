using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Constants;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BlogFlow.API.QueryExtensions;
using BlogFlow.API.Exceptions;
using BlogFlow.API.Domain.Entities;

namespace BlogFlow.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository repo, ILogger<CategoryService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryReadDTO>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all categories");

            var categories = await _repo.GetCategoriesQuery()
                .AsDTO()
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Fetched {Count} categories", categories.Count);

            return categories;
        }

        public async Task<CategoryReadDTO?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching category by id {CategoryId}", id);

            var category = await _repo.GetCategoryQuery(id)
                .AsDTO()
                .FirstOrDefaultAsync(cancellationToken);

            if (category is null)
            {
                _logger.LogWarning("Category not found: {CategoryId}", id);
                throw new NotFoundException("Category", id);
            }

            return category;
        }

        public async Task<CategoryReadDTO> CreateCategoryAsync(CategoryCreateDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating category with name {CategoryName}", dto.Name);

            if (await _repo.ExistsByNameAsync(dto.Name, cancellationToken: cancellationToken))
            {
                _logger.LogWarning("Create category failed - name already exists: {CategoryName}", dto.Name);
                throw new ConflictException(
                    $"Category '{dto.Name}' already exists.",
                    "CATEGORY_ALREADY_EXISTS");
            }

            var category = new Category(dto.Name);

            await _repo.CreateCategoryAsync(category, cancellationToken);
            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Category created successfully: {CategoryId} ({DisplayName})",
                category.Id,
                category.DisplayName);

            return category.ToDTO();
        }

        public async Task<CategoryReadDTO> RenameCategoryAsync(Guid id, string newName, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Renaming category {CategoryId} to {NewName}", id, newName);

            var category = await _repo.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException("Category", id);

            if (await _repo.ExistsByNameAsync(newName, id, cancellationToken))
            {
                _logger.LogWarning(
                    "Rename failed - duplicate name {NewName} for category {CategoryId}",
                    newName,
                    id);

                throw new ConflictException(
                    $"A category with the name '{newName}' already exists.",
                    "CATEGORY_NAME_CONFLICT");
            }

            var oldName = category.DisplayName;

            category.Rename(newName);

            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Category renamed: {CategoryId} from '{OldName}' to '{NewName}'",
                id,
                oldName,
                newName);

            return category.ToDTO();
        }

        public async Task<DeleteCategoryResultDTO> DeleteCategoryAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            if (id == CategoryConstants.UncategorizedId)
                throw new BadRequestException(
                    "The Uncategorized category cannot be deleted.",
                    "CATEGORY_IS_PROTECTED");

            var category = await _repo.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException("Category", id);

            var reassignedCount = await _repo.ReassignPostsAsync(
                id,
                CategoryConstants.UncategorizedId,
                cancellationToken);

            await _repo.DeleteAsync(category, cancellationToken);
            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Category deleted: {CategoryId} ({DisplayName}). {ReassignedCount} posts reassigned to Uncategorized.",
                id,
                category.DisplayName,
                reassignedCount);

            return new DeleteCategoryResultDTO
            {
                DeletedCategoryId = id,
                DeletedCategoryName = category.DisplayName,
                ReassignedPostCount = reassignedCount
            };
        }
    }
}