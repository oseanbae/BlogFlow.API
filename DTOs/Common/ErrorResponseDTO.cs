namespace BlogFlow.API.DTOs.Common
{
    public sealed class ErrorResponseDTO
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = default!;
        public string? ErrorCode { get; init; }
        public DateTimeOffset Timestamp { get; init; }
        public string? TraceId { get; init; }
        public IEnumerable<string>? Errors { get; init; }
    }
}
