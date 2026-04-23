using System.Text.Json.Serialization;

namespace BlogFlow.API.Models
{
    public enum UserRole
    {
        Admin = 1,
        Author = 2,
        Reader = 3
    }

    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public UserRole Role { get; set; } = UserRole.Reader;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; private set; }
        public void SoftDelete()
        {
            if (DeletedAt != null) throw new InvalidOperationException("This user is already deleted.");
            DeletedAt = DateTime.UtcNow;
        }
        public void Restore()
        {
           if (DeletedAt == null) throw new InvalidOperationException("This user is not deleted.");
           DeletedAt = null;
        }
        
        // Navigation properties
        [JsonIgnore]
        public ICollection<Post> Posts { get; set; } = [];

        [JsonIgnore]
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

        [JsonIgnore]
        public ICollection<Comment> Comments { get; set; } = [];
    }
}