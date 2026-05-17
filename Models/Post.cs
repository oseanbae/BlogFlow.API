using BlogFlow.API.Exceptions;

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
                throw new BadRequestException("Title is required", "EMPTY_POST_TITLE");

            if (string.IsNullOrWhiteSpace(body))
                throw new BadRequestException("Body is required", "EMPTY_POST_BODY");

            if (authorId == Guid.Empty)
                throw new BadRequestException("Invalid author", "INVALID_AUTHOR_ID");

            Title = title.Trim();
            Body = body;
            AuthorId = authorId;
            CategoryId = categoryId;
        }

        // SEED constructor
        public Post(Guid id, string title, string body, Guid authorId, Guid categoryId)
            : this(title, body, authorId, categoryId)
        {
            Id = id;
        }

        public void SoftDelete()
        {
            if (DeletedAt != null) throw new ConflictException($"Post '{Id}' is already deleted.", "POST_ALREADY_DELETED");
            DeletedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (DeletedAt == null) throw new ConflictException($"Post '{Id}' is not deleted.", "POST_NOT_DELETED");
            DeletedAt = null;
        }
        public void Update(string title, string body, Guid categoryId)
        {
            if (DeletedAt != null)
                throw new BadRequestException("Cannot update a deleted post.", "POST_ALREADY_DELETED");

            if (string.IsNullOrWhiteSpace(title))
                throw new BadRequestException("Title is required", "EMPTY_POST_TITLE");

            if (string.IsNullOrWhiteSpace(body))
                throw new BadRequestException("Body is required", "EMPTY_POST_BODY");

            if (categoryId == Guid.Empty)
                throw new BadRequestException("Invalid category", "INVALID_CATEGORY_ID");

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
