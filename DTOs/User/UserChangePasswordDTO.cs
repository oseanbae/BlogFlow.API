namespace BlogFlow.API.DTOs.User
{
    public class UserChangePasswordDTO
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
