using BlogFlow.API.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.QueryExtensions
{
    public static class Pagination
    {
        public static async Task<PaginatedResultDTO<T>> ToPaginatedResultAsync<T>(
            this IQueryable<T> query,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

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