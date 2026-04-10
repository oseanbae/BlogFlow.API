using System.ComponentModel.DataAnnotations;

namespace BlogFlow.API.DTOs.Auth
{
    public class RegisterRequestDTO
    {
        [Required, StringLength(50, MinimumLength = 3)]
        public required string Username { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public required string Email { get; set; }

        [Required, StringLength(255, MinimumLength = 6)]
        public required string Password { get; set; }
    }
}