namespace BlogFlow.API.DTOs.Comment
{
    public class CommentReadDTO
    {
        public Guid Id { get; init; }
        public Guid PostId { get; init; }
        public Guid UserId { get; init; }
        public string AuthorName { get; init; } = null!;
        public string Body { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}