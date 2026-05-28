using BlogFlow.API.Domain.Entities;

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