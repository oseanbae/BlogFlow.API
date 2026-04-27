using BlogFlow.API.Models;
using BlogFlow.API.Services.Interfaces;
using System.Security.Claims;

namespace BlogFlow.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public CurrentUserService(IHttpContextAccessor accessor)
        {
            _contextAccessor = accessor;
        }

        public Guid? GetSubClaimUserId()
        {
            var subClaim = _contextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(subClaim, out var userId))
                return null;

            return userId;
        }

        public Guid GetRequiredUserId()
        {
            return GetSubClaimUserId()
                ?? throw new InvalidOperationException("User is not authenticated.");
        }

        public UserRole GetRole()
        {
            var roleClaim = _contextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.Role)?.Value;

            if (Enum.TryParse<UserRole>(roleClaim, ignoreCase: true, out var role))
                return role;

            return UserRole.Reader;
        }
        public UserContext GetCurrentUser()
        {
            return new UserContext
            {
                UserId = GetSubClaimUserId(),
                Role = GetRole()
            };
        }

    }
}