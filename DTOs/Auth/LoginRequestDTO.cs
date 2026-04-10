using System.ComponentModel.DataAnnotations;

namespace BlogFlow.API.DTOs.Auth
{
    public class LoginRequestDTO
    {
        [Required, EmailAddress, StringLength(100)]
        public required string Email { get; set; }

        [Required, StringLength(255, MinimumLength = 6)]
        public required string Password { get; set; }
    }
}
