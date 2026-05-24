namespace BlogFlow.API.DTOs.User
{
    public class UserChangePasswordDTO
    {
        public string CurrentPassword { get; init; } = null!;
        public string NewPassword { get; init; } = null!;
    }
}
