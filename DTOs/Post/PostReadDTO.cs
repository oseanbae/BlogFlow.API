using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Models;

namespace BlogFlow.API.DTOs.Post
{
    public class PostReadDTO
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public string Body { get; init; } = null!;
        public PostState State { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public Guid AuthorId { get; init; }
        public string AuthorUsername { get; init; } = null!;
        public Guid CategoryId { get; init; }
        public string CategoryName { get; init; } = null!;
        public IEnumerable<TagReadDTO> Tags { get; init; } = [];
    }   
}
