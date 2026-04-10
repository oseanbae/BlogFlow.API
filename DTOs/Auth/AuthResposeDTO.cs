using BlogFlow.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design.Serialization;

namespace BlogFlow.API.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public Guid Id { get; set; }

        [Required, StringLength(50, MinimumLength = 3)]
        public required string Username { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public required string Email { get; set; }

        [Required, StringLength(20)]
        public required UserRole Role { get; set; }

        public required string Token { get; set; } // JWT
    }
}
