using BlogFlow.API.Models;
using System.Text.Json.Serialization;

namespace BlogFlow.API.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }

        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime RefreshTokenExpiry { get; set; }
    }
}