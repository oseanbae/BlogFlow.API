using BlogFlow.API.Exceptions.Base;

namespace BlogFlow.API.Exceptions
{
    public sealed class ForbiddenException : AppException
    {
        public ForbiddenException(
            string message = "You do not have permission to perform this action.",
            string errorCode = "ACCESS_FORBIDDEN")
            : base(message, errorCode, StatusCodes.Status403Forbidden)
        {
        }
    }
}
