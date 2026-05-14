namespace BlogFlow.API.Exceptions.Base
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }
        public DateTimeOffset Timestamp { get; }
        public string? TraceId { get; set; }
        public IEnumerable<string>? Errors { get; }

        protected AppException(
                string message,
                string errorCode = "GENERIC_ERROR",
                int statusCode = 500, //default interal server
                IEnumerable<string>? errors = null) : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            Timestamp = DateTimeOffset.UtcNow;
            Errors = errors;
        } 
    }
}