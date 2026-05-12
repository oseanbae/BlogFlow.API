using BlogFlow.API.Models;

namespace BlogFlow.API.DTOs.User
{
    public class UserReadDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
