using System.ComponentModel.DataAnnotations;

namespace BlogFlow.API.DTOs.Auth
{
    public class RefreshTokenRequestDTO
    {
        [Required]
        public required string RefreshToken { get; set; }
    }
}
