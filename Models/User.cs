using BlogFlow.API.Exceptions;
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
            if (DeletedAt != null) throw new ConflictException($"User '{Username}' is already deleted.", "USER_ALREADY_DELETED");
            DeletedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (DeletedAt == null) throw new ConflictException($"User '{Username}' is not deleted.", "USER_NOT_DELETED");
            DeletedAt = null;
        }

        public void UpdateIdentity(string newUsername, string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newUsername))
                throw new BadRequestException("Username cannot be null or whitespace.", "EMPTY_USERNAME");

            if (string.IsNullOrWhiteSpace(newEmail))
                throw new BadRequestException("Email cannot be null or whitespace.", "EMPTY_EMAIL");

            if (!newEmail.Contains('@'))
                throw new BadRequestException("Invalid email format.", "INVALID_EMAIL_FORMAT");

            if (Username == newUsername && Email == newEmail)
                return;

            Username = newUsername;
            Email = newEmail;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangePassword(string newHashedPassword)
        {
            if (string.IsNullOrWhiteSpace(newHashedPassword))
                throw new BadRequestException("Password hash cannot be empty.", "EMPTY_PASSWORD_HASH");

            PasswordHash = newHashedPassword;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateRole(UserRole newRole)
        {
            if (Role == newRole) throw new ConflictException($"User is already assigned the {newRole} role.", "ROLE_ALREADY_ASSIGNED");

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