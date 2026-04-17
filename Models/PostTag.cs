namespace BlogFlow.API.Models
{
    // Join entity for many-to-many relationship between Post and Tag
    public class PostTag
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;
        public Guid TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}
