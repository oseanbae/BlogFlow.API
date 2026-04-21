namespace BlogFlow.API.DTOs.Auth
{
    public class RegisterRequestDTO
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}