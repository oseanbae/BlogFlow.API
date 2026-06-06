using BlogFlow.API.Exceptions;
using Newtonsoft.Json.Bson;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace BlogFlow.API.Domain.Entities;

public enum UserRole
{
    Admin = 1,
    Author = 2,
    Reader = 3
}

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; } = UserRole.Reader;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public User() { } //EF Core
    public User(string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new BadRequestException("Username cannot be null or whitespace.", "EMPTY_USERNAME");

        if (string.IsNullOrWhiteSpace(email))
            throw new BadRequestException("Email cannot be null or whitespace.", "EMPTY_EMAIL");

        if (!email.Contains('@'))
            throw new BadRequestException("Invalid email format.", "INVALID_EMAIL_FORMAT");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new BadRequestException("Password hash cannot be empty.", "EMPTY_PASSWORD_HASH");

        Username = username.Trim();
        Email = email.Trim();
        PasswordHash = passwordHash;
    }

    // SEED Constructor
    public User(Guid id, string username, string email, string passwordHash, UserRole role)
    : this(username, email, passwordHash)
    {
        Id = id;
        Role = role;
    }

    public void SoftDelete()
    {
        if (DeletedAt != null) throw new ConflictException($"User '{Username}' is already deleted.", "USER_ALREADY_DELETED");
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Restore()
    {
        if (DeletedAt == null) throw new ConflictException($"User '{Username}' is not deleted.", "USER_NOT_DELETED");
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
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
    public void RehashPassword(string newHashedPassword)
    {
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