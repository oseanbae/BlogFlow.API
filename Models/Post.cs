using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;

namespace BlogFlow.API.Models
{
    public class Post
    {
        //PK
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Title { get; private set; } = null!;
        public string Body { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        //[FK]
        public Guid AuthorId { get; private set; }
        public Guid CategoryId { get; private set; }

        //Navigation Properties
        public User Author { get; private set; } = null!;
        public Category Category { get; private set; } = null!;
        public ICollection<PostTag> PostTags { get; private set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];

        //Constructor for EF Core to load data from db
        private Post() { }

        public Post(string title, string body, Guid authorId, Guid categoryId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body is required");

            if (authorId == Guid.Empty)
                throw new ArgumentException("Invalid author");

            Title = title.Trim();
            Body = body;
            AuthorId = authorId;
            CategoryId = categoryId;
        }

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
            if (DeletedAt != null)
                throw new InvalidOperationException("Cannot update a deleted post.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body is required");

            if (categoryId == Guid.Empty)
                throw new ArgumentException("Invalid category");

            Title = title.Trim();
            Body = body;
            CategoryId = categoryId;
            UpdatedAt = DateTime.UtcNow;
        }
        public void SetTags(IEnumerable<Guid> tagIds)
        {
            if (tagIds == null)
                return;

            var newTagIds = tagIds.Distinct().ToHashSet();
            var existingTagIds = PostTags.Select(pt => pt.TagId).ToHashSet();

            var toRemove = PostTags
                .Where(pt => !newTagIds.Contains(pt.TagId))
                .ToList();

            foreach (var tag in toRemove)
            {
                PostTags.Remove(tag);
            }

            var toAdd = newTagIds
                .Where(id => !existingTagIds.Contains(id));

            foreach (var tagId in toAdd)
            {
                PostTags.Add(new PostTag(Id, tagId));
            }
        }
    }
}
