namespace BlogFlow.API.DTOs.Post
{
    public class PostCreateDTO
    {
        public required string Title { get; init; }
        public required string Body { get; init; }
        public Guid CategoryId { get; init; }
        public List<Guid>? TagIds { get; init; }
    }
}
