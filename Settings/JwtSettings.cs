namespace BlogFlow.API.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; } =  string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public double AccessTokenExpiryMinutes { get; set; }
        public double RefreshTokenExpiryDays { get; set; }
        
    }
}
