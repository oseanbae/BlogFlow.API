namespace BlogFlow.API.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; } =  string.Empty;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public double AccessTokenExpiryMinutes { get; set; }
        public double RefreshTokenExpiryDays { get; set; }
        
    }
}
