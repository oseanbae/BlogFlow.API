namespace BlogFlow.API.DTOs.Auth
{
    public class RevokeRequestDTO
    {
        public required string RefreshToken { get; set; }
    }
}
