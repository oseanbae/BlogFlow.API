using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface ICurrentUserService
    {
        UserContext GetCurrentUser();
        UserContext GetRequiredUser();
        Guid GetRequiredUserId();
        UserRole GetRequiredValidRole();
    }
}