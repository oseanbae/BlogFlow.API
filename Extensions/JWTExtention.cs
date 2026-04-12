using BlogFlow.API.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class JWTExtension
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection("JwtSettings"))
            .Validate(jwt => !string.IsNullOrWhiteSpace(jwt.Secret), "JWT Secret is missing")
            .Validate(jwt => !string.IsNullOrWhiteSpace(jwt.Issuer), "JWT Issuer is missing")
            .Validate(jwt => !string.IsNullOrWhiteSpace(jwt.Audience), "JWT Audience is missing")
            .Validate(jwt => jwt.AccessTokenExpiryMinutes > 0, "Invalid AccessTokenExpiryMinutes")
            .Validate(jwt => jwt.RefreshTokenExpiryDays > 0, "Invalid RefreshTokenExpiryDays")
            .ValidateOnStart();

        var settings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new Exception("JwtSettings is missing");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret))
                };
            });

        services.AddAuthorization();

        return services;
    }
}