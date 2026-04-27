using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? GetSubClaimUserId();
        Guid GetRequiredUserId();
        UserRole GetRole();
        UserContext GetCurrentUser();
    }
}
