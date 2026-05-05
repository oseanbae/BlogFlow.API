namespace BlogFlow.API.DTOs.Comment
{
    public class CommentReadDTO
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string AuthorName { get; set; } = null!;
        public string Body { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}