using System.ComponentModel.DataAnnotations;

namespace BlogFlow.API.DTOs.Auth
{
    public class RevokeRequestDTO
    {
        [Required]
        public required string RefreshToken { get; set; }
    }
}
