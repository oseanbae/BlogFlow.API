namespace BlogFlow.API.Queries
{
    public static class Pagination
    {
        public static IQueryable<T> ApplyPagination<T>(
            this IQueryable<T> query,
            int page,
            int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            int skip = (page - 1) * pageSize;

            return query.Skip(skip).Take(pageSize);
        }
    }
}