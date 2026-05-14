using BlogFlow.API.Exceptions.Base;

namespace BlogFlow.API.Exceptions
{
    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(
            string message = "You must be logged in to access this resource.",
            string errorCode = "UNAUTHORIZED")
            : base(message, errorCode, StatusCodes.Status401Unauthorized)
        {}
    }
}
