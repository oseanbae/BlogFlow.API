namespace BlogFlow.API.Models
{
    public class Comment
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Body { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public Guid UserId { get; }
        public Guid PostId { get; }

        //Navigation Properties
        public User User { get; } = null!; 
        public Post Post { get; } = null!;

        public Comment() { } //EF Core

        public Comment(string body, Guid userId, Guid postId)
        {
            if (userId == Guid.Empty) throw new ArgumentException("User ID is required.");
            if (postId == Guid.Empty) throw new ArgumentException("Post ID is required.");

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Comment body cannot be empty.");

            Body = body.Trim();

            UserId = userId;
            PostId = postId;
        }

        public void UpdateBody(string newBody)
        {
            if (string.IsNullOrWhiteSpace(newBody))
                throw new ArgumentException("Comment body cannot be empty.");

            var sanitizedBody = newBody.Trim();

            //check if it actually changed
            if (Body == sanitizedBody) return;

            Body = sanitizedBody;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            if (DeletedAt != null) throw new InvalidOperationException("This comment is already deleted.");
            DeletedAt = DateTime.UtcNow;
        }
        public void Restore()
        {
            if (DeletedAt == null) throw new InvalidOperationException("This comment is not deleted.");
            DeletedAt = null;
        }
    }
}