namespace BlogFlow.API.Models
{
    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Body { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        //Navigation Properties
        public User User { get; set; } = null!; 
        public Post Post { get; set; } = null!;

    }
}
