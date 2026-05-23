namespace BlogFlow.API.Models
{
    public class UserContext
    {
        public Guid UserId { get; init; }
        public UserRole? Role { get; init; } 
        public bool IsAdmin => Role == UserRole.Admin;
        public bool IsAuthor => Role == UserRole.Author;
        public bool IsAuthenticated => UserId != Guid.Empty && Role.HasValue;
    }
}
