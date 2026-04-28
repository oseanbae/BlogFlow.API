using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ICurrentUserService
    {
        Guid GetRequiredUserId();
        UserRole GetRole();
        UserContext GetCurrentUser();
    }
}
