using BlogFlow.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Queries
{
    public static class Pagination
    {
        public static async Task<PaginatedResultDTO<T>> ToPaginatedResultAsync<T>(
            this IQueryable<T> query,
            int page,
            int pageSize)
        {
            // 1. Sanitize inputs
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            // 2. Get the total count (First DB hit)
            var totalCount = await query.CountAsync();

            // 3. Get the items (Second DB hit)
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