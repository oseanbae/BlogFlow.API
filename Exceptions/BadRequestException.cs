using BlogFlow.API.Exceptions.Base;

namespace BlogFlow.API.Exceptions
{
    public sealed class BadRequestException : AppException
    {
        public BadRequestException(string message, string errorCode = "BAD_REQUEST")
            : base(message, errorCode, StatusCodes.Status400BadRequest)
        { }
    }
}
