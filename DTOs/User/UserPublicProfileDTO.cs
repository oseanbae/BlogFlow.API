using BlogFlow.API.Models;

namespace BlogFlow.API.DTOs.User
{
    public class UserPublicProfileDTO
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = null!;
        public UserRole Role { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
