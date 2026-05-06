namespace BlogFlow.API.Models
{
    public class PostQueryParams
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? TagId { get; set; }
        public string? Keyword { get; set; }
    }
}
