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
            var user = _contextAccessor.HttpContext?.User;

            var subClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(subClaim, out var userId))
                return null;

            return userId;
        }

        public Guid GetRequiredUserId()
        {
            var userId = GetSubClaimUserId();

            if (userId == null)
                throw new InvalidOperationException("User is not authenticated.");

            return userId.Value;
        }
    }
}
