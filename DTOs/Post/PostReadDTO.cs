using BlogFlow.API.DTOs.Tag;

namespace BlogFlow.API.DTOs.Post
{
    public class PostReadDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorUsername { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public List<TagReadDTO> Tags { get; set; } = [];
    }
}
