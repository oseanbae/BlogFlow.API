using BlogFlow.API.DTOs.Categories;
using Microsoft.EntityFrameworkCore;
namespace BlogFlow.API.QueryExtensions
{
    public static class CategoryQueryExtensions
    {
        public static IQueryable<CategoryReadDTO> AsDTO(this IQueryable<Category> query)
        {
            return query
                .AsNoTracking()
                .Select(c => new CategoryReadDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                });
        }

        public static CategoryReadDTO ToDTO(this Category category)
        {
            return new CategoryReadDTO
            {
                Id = category.Id,
                Name = category.Name,
            };
        }
    }
}
