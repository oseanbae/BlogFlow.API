using BlogFlow.API.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.QueryExtensions
{
    public static class Pagination
    {
        public static async Task<PaginatedResultDTO<T>> ToPaginatedResultAsync<T>(
            this IQueryable<T> query,
            int page,
            int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResultDTO<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}