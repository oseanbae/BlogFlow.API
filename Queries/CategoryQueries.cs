using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.Models;

namespace BlogFlow.API.Queries
{
    public static class CategoryQueries
    {
        public static IQueryable<CategoryReadDTO> AsDTO(this IQueryable<Category> query)
        {
            return query.Select(c => new CategoryReadDTO
            {
                Id = c.Id,
                Name = c.Name,
            });
        }
    }
}
