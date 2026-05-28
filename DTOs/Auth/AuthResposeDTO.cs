using BlogFlow.API.Domain.Entities;
using System.Text.Json.Serialization;

namespace BlogFlow.API.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = null!;
        public string Email { get; init; } = null!;

        public UserRole Role { get; init; }

        public string AccessToken { get; init; } = null!;
        public string RefreshToken { get; init; } = null!;
        public DateTime RefreshTokenExpiry { get; init; }
    }
}