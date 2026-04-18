using NuGet.Protocol.Plugins;

namespace BlogFlow.API.Models
{
    public class Post
    {
        //PK
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Title { get; set; }
        public required string Body { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        //[FK]
        public Guid AuthorId { get; private set; }
        public Guid CategoryId { get; private set; }

        //Navigation Properties
        public User Author { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<PostTag> PostTags { get; set; } = [];
        public void SoftDelete()
        {
            if (DeletedAt != null) throw new InvalidOperationException("This post is already deleted.");
            DeletedAt = DateTime.UtcNow;
        }
        public void Restore()
        {
            if (DeletedAt == null) throw new InvalidOperationException("This post is not deleted.");
            DeletedAt = null;
        }

        public void Update(string title, string body, Guid categoryId)
        {
            Title = title;
            Body = body;
            CategoryId = categoryId;
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
