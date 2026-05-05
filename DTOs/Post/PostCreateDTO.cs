namespace BlogFlow.API.DTOs.Post
{
    public class PostCreateDTO
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
        public Guid CategoryId { get; set; }
        public List<Guid>? TagIds { get; set; }
    }
}
