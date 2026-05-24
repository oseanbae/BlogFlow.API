namespace BlogFlow.API.DTOs.Common
{
    public class PaginatedResultDTO<T>
    {
        public List<T> Items { get; init; } = [];
        public int TotalCount { get; init; }
        public int Page { get; init; }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 1 : value;
        }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}