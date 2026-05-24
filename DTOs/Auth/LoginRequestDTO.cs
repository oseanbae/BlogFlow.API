namespace BlogFlow.API.DTOs.Auth
{
    public class LoginRequestDTO
    {
        public string UsernameOrEmail { get; init; } = null!;
        public required string Password { get; init; }
    }
}
