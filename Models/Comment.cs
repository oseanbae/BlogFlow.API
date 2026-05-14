using BlogFlow.API.Exceptions;

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
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID is required.", "INVALID_USER_ID");

            if (postId == Guid.Empty)
                throw new BadRequestException("Post ID is required.", "INVALID_POST_ID");

            if (string.IsNullOrWhiteSpace(body))
                throw new BadRequestException("Comment body cannot be empty.", "EMPTY_COMMENT_BODY");

            Body = body.Trim();
            UserId = userId;
            PostId = postId;
        }

        public void UpdateBody(string newBody)
        {
            if (string.IsNullOrWhiteSpace(newBody))
                throw new BadRequestException("Comment body cannot be empty.", "EMPTY_COMMENT_BODY");

            var sanitizedBody = newBody.Trim();

            // Check if it actually changed
            if (Body == sanitizedBody) return;

            Body = sanitizedBody;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            if (DeletedAt != null) throw new ConflictException($"Comment '{Id}' is already deleted.", "COMMENT_ALREADY_DELETED");
            DeletedAt = DateTime.UtcNow;
        }

        public void Restore()   
        {
            if (DeletedAt == null) throw new ConflictException($"Comment '{Id}' is not deleted.", "COMMENT_NOT_DELETED");
            DeletedAt = null;
        }
    }
}   