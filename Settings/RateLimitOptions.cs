namespace BlogFlow.API.Settings
{
    public class RateLimitOptions
    {
        public const string SectionName = "RateLimiting";

        public RateLimitPolicyOptions Register { get; set; } = new();
        public RateLimitPolicyOptions Login { get; set; } = new();
        public RateLimitPolicyOptions Refresh { get; set; } = new();
        public RateLimitPolicyOptions Revoke { get; set; } = new();
        
    }
    public class RateLimitPolicyOptions
    {
        public int MaxRequest { get; set; } = 1; // Default to 1
        public int WindowSecond { get; set; } = 60;
        public int QueueLimit { get; set; } = 0;
        public bool AutoReplenishment { get; set; } = true;
    }
}
