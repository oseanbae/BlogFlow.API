using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlogFlow.API.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Admin = 1,
        Author = 2,
        Reader = 3
    }

    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        [Required]
        public UserRole Role { get; set; } = UserRole.Reader;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        [JsonIgnore]
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}