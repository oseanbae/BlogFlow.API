namespace BlogFlow.API.DTOs.Post
{
    public class PostUpdateDTO
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
        public Guid? CategoryId { get; set; }
        public List<Guid>? TagIds { get; set; }
    }
}
