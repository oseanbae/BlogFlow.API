using System.Threading.RateLimiting;
using BlogFlow.API.Settings;
namespace BlogFlow.API.Extensions
{
    public static class RateLimitingExtension
    {
        public static IServiceCollection AddAuthRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<RateLimitOptions>()
                .Bind(configuration.GetSection(RateLimitOptions.SectionName))
                .Validate(options =>
                    options.Login.MaxRequest > 0 &&
                    options.Register.MaxRequest > 0 &&
                    options.Refresh.MaxRequest > 0 &&
                    options.Revoke.MaxRequest > 0,
                    "All MaxRequest values must be greater than 0"
                )
                .ValidateOnStart();
            var rateLimitConfig = configuration
                .GetSection(RateLimitOptions.SectionName)
                .Get<RateLimitOptions>() ?? new RateLimitOptions();

            services.AddRateLimiter(options =>
            {
                options.AddPolicy("register", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimitConfig.Register.MaxRequest,
                            Window = TimeSpan.FromSeconds(rateLimitConfig.Register.WindowSecond),
                            QueueLimit = rateLimitConfig.Register.QueueLimit,
                            AutoReplenishment = rateLimitConfig.Register.AutoReplenishment
                        }));

                options.AddPolicy("login", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                         partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                         factory: _ => new FixedWindowRateLimiterOptions
                         {
                             PermitLimit = rateLimitConfig.Login.MaxRequest,
                             Window = TimeSpan.FromSeconds(rateLimitConfig.Login.WindowSecond),
                             QueueLimit = rateLimitConfig.Login.QueueLimit,
                             AutoReplenishment = rateLimitConfig.Login.AutoReplenishment
                         }));

                options.AddPolicy("refresh", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimitConfig.Refresh.MaxRequest,
                            Window = TimeSpan.FromSeconds(rateLimitConfig.Refresh.WindowSecond),
                            QueueLimit = rateLimitConfig.Refresh.QueueLimit,
                            AutoReplenishment = rateLimitConfig.Refresh.AutoReplenishment
                        }));

                options.AddPolicy("revoke", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitConfig.Revoke.MaxRequest,
                        Window = TimeSpan.FromSeconds(rateLimitConfig.Revoke.WindowSecond),
                        QueueLimit = rateLimitConfig.Revoke.QueueLimit,
                        AutoReplenishment = rateLimitConfig.Revoke.AutoReplenishment
                    }));

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "Too many attempts",
                        message = "Please try again later",
                        retryAfter = 60
                    }, cancellationToken: token);
                };
            });

            return services;
        }
    }
}
