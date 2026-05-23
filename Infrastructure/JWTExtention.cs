using BlogFlow.API.DTOs.Common;
using BlogFlow.API.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class JWTExtension
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind JwtSettings from configuration (appsettings.json)
        // and register it in the Options pipeline
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection("JwtSettings"))

            // Validate required fields to prevent invalid runtime behavior
            .Validate(jwt => !string.IsNullOrWhiteSpace(jwt.Secret), "JWT Secret is missing")
            .Validate(jwt => !string.IsNullOrWhiteSpace(jwt.Issuer), "JWT Issuer is missing")
            .Validate(jwt => !string.IsNullOrWhiteSpace(jwt.Audience), "JWT Audience is missing")
            .Validate(jwt => jwt.AccessTokenExpiryMinutes > 0, "Invalid AccessTokenExpiryMinutes")
            .Validate(jwt => jwt.RefreshTokenExpiryDays > 0, "Invalid RefreshTokenExpiryDays")

            // Ensures validation runs at application startup (fail fast if invalid)
            .ValidateOnStart();

        // Register authentication services and set JWT as the default scheme
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Register JWT Bearer handler WITHOUT inline configuration
            // We defer configuration to use the validated JwtSettings via the Options pipeline
            .AddJwtBearer();

        // Configure JwtBearerOptions using deferred execution
        // This runs AFTER JwtSettings has been validated and is resolved via DI
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtSettings>>((bearerOptions, jwtSettings) =>
            {
                var s = jwtSettings.Value; // This is already validated

                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    // Ensure token issuer matches expected value
                    ValidateIssuer = true,

                    // Ensure token audience matches expected value
                    ValidateAudience = true,

                    // Ensure token is not expired
                    ValidateLifetime = true,

                    // Ensure token signature is valid
                    ValidateIssuerSigningKey = true,

                    // Values come from validated JwtSettings
                    ValidIssuer = s.Issuer,
                    ValidAudience = s.Audience,

                    // Symmetric key used to validate token signature
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(s.Secret))
                };

                bearerOptions.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        // Prevents default empty response
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new ErrorResponseDTO
                        {
                            StatusCode = StatusCodes.Status401Unauthorized,
                            Message = "Authentication failed. Please provide a valid access token.",
                            ErrorCode = "UNAUTHORIZED",
                            Timestamp = DateTimeOffset.UtcNow,
                            TraceId = context.HttpContext.TraceIdentifier,
                            Errors = null
                        };

                        await context.Response.WriteAsJsonAsync(response);
                    }
                };
            });
            
        // Register authorization services (used with [Authorize] attributes)
        services.AddAuthorization();

        return services;
    }
}