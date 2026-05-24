using BlogFlow.API.DTOs.Categories;
using Microsoft.EntityFrameworkCore;
using BlogFlow.API.Models;

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
                    DisplayName = c.DisplayName
                });
        }

        public static CategoryReadDTO ToDTO(this Category category)
        {
            return new CategoryReadDTO
            {
                Id = category.Id,
                Name = category.Name,
                DisplayName = category.DisplayName
            };
        }
    }
}
