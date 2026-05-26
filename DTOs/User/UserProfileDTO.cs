using BlogFlow.API.Models;

namespace BlogFlow.API.DTOs.User
{
    public class UserProfileDTO
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = null!;
        public string Email { get; init; } = null!;
        public UserRole Role { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
