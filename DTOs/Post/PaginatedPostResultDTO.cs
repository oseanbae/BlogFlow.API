namespace BlogFlow.API.DTOs.Post
{
    public class PaginatedPostResultDTO
    {
        public ICollection<PostReadDTO> Items { get; set; } = null!;
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int) Math.Ceiling((double) TotalCount / PageSize);

    }
}
