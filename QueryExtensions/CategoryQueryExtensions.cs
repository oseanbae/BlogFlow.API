using BlogFlow.API.DTOs.Categories;

namespace BlogFlow.API.QueryExtensions
{
    public static class CategoryQueryExtensions
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
