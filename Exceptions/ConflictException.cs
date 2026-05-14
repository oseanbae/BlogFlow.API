using BlogFlow.API.Exceptions.Base;

namespace BlogFlow.API.Exceptions
{
    public sealed class ConflictException : AppException
    {
        public ConflictException(
            string message,
            string errorCode = "RESOURCE_CONFLICT")
            : base(message, errorCode, StatusCodes.Status409Conflict)
        {}
    }
}
