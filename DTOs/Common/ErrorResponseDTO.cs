namespace BlogFlow.API.DTOs.Common
{
    public sealed class ErrorResponseDTO
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = default!;
        public string? ErrorCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string? TraceId { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
