namespace BlogFlow.API.DTOs.Post
{
    public class PaginatedPostResultDTO
    {
        public IEnumerable<PostReadDTO> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int) Math.Ceiling((double) TotalCount / PageSize);

    }
}
