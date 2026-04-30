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

        public Guid GetRequiredUserId()
        {
            var subClaim = _contextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(subClaim, out var userId))
                throw new InvalidOperationException("Invalid or missing user id claim.");

            return userId;
        }
        public UserRole GetRole()
        {
            var roleClaim = (_contextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.Role)?.Value) ?? 
                throw new InvalidOperationException("Missing role claim.");

            if (!Enum.TryParse<UserRole>(roleClaim, ignoreCase: true, out var role))
                throw new InvalidOperationException("Invalid role claim");

            return role;
        }

        public UserContext GetCurrentUser()
        {
            return new UserContext
            {
                UserId = GetRequiredUserId(),
                Role = GetRole()
            };
        }

    }
}