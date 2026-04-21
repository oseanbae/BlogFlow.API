namespace BlogFlow.API.DTOs.Auth
{
    public class LoginRequestDTO
    {
        public string UsernameOrEmail { get; set; } = null!;
        public required string Password { get; set; }
    }
}
