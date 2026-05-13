using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json.Bson;
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
        public DateTime? UpdatedAt { get; private set; }
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

        public void UpdateIdentity(string newUsername, string newEmail)
        {
           
            if (string.IsNullOrWhiteSpace(newUsername))
                throw new ArgumentException("Username cannot be null or whitespace.", nameof(newUsername));

            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException("Email cannot be null or whitespace.", nameof(newEmail));

            if (!newEmail.Contains('@'))
                throw new ArgumentException("Invalid email format.", nameof(newEmail));

            if (Username == newUsername && Email == newEmail)
                return;

            Username = newUsername;
            Email = newEmail;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangePassword(string newHashedPassword)
        {
            if (string.IsNullOrWhiteSpace(newHashedPassword))
                throw new ArgumentException("Password hash cannot be empty.", nameof(newHashedPassword));

            PasswordHash = newHashedPassword;
            UpdatedAt = DateTime.UtcNow; 
        }
        public void UpdateRole(UserRole newRole)
        {
            if (Role == newRole)
                throw new InvalidOperationException($"User is already assigned the {newRole} role.");

            Role = newRole;
            UpdatedAt = DateTime.UtcNow;
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