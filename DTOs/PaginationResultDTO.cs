namespace BlogFlow.API.DTOs
{
    public class PaginatedResultDTO<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 1 : value;
        }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}