using BlogFlow.API.Exceptions;
using BlogFlow.API.Models;
using BlogFlow.API.Services.Interfaces;
using System.Security.Claims;

namespace BlogFlow.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(
            IHttpContextAccessor contextAccessor,
            ILogger<CurrentUserService> logger)
        {
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Returns the current user context.
        /// Returns an empty UserContext when the request is unauthenticated.
        /// </summary>
        public UserContext GetCurrentUser()
        {
            var user = _contextAccessor.HttpContext?.User;

            // Guest / unauthenticated request
            if (user?.Identity?.IsAuthenticated != true)
            {
                return new UserContext();
            }

            // Required user id claim
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning(
                    "Authenticated request contains invalid user id claim");

                return new UserContext();
            }

            // Optional role claim
            var roleClaim = user.FindFirstValue(ClaimTypes.Role);

            UserRole? parsedRole = null;

            if (Enum.TryParse<UserRole>(roleClaim, true, out var role))
            {
                parsedRole = role;
            }
            else if (!string.IsNullOrWhiteSpace(roleClaim))
            {
                _logger.LogWarning(
                    "Authenticated request contains invalid role claim: {RoleClaim}",
                    roleClaim);
            }

            return new UserContext
            {
                UserId = userId,
                Role = parsedRole
            };
        }

        /// <summary>
        /// Returns the authenticated user.
        /// Throws if the request is unauthenticated.
        /// </summary>
        public UserContext GetRequiredUser()
        {
            var user = GetCurrentUser();

            if (!user.IsAuthenticated)
            {
                _logger.LogWarning(
                    "Unauthorized access attempt to authenticated resource");

                throw new UnauthorizedException(
                    "Authentication required.",
                    "AUTH_REQUIRED"
                );
            }

            return user;
        }

        /// <summary>
        /// Returns the authenticated user's id.
        /// Throws if the request is unauthenticated.
        /// </summary>
        public Guid GetRequiredUserId()
        {
            return GetRequiredUser().UserId;
        }

        /// <summary>
        /// Returns the authenticated user's valid role.
        /// Throws if the role claim is missing or invalid.
        /// </summary>
        public UserRole GetRequiredValidRole()
        {
            var user = GetRequiredUser();

            if (user.Role is null)
            {
                _logger.LogWarning(
                    "Authenticated user {UserId} has missing or invalid role claim",
                    user.UserId);

                throw new ForbiddenException(
                    "User role claim is missing or invalid.",
                    "INVALID_ROLE_CLAIM"
                );
            }

            return user.Role.Value;
        }
    }
}