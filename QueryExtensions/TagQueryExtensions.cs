using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.QueryExtensions
{
    public static class TagQueryExtensions
    {
        public static IQueryable<TagReadDTO> AsDTO(this IQueryable<Tag> query)
        {
            return query
                .AsNoTracking()
                .Select(c => new TagReadDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    DisplayName = c.DisplayName
                });
        }
    }
}
