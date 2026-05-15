using BlogFlow.API.Settings;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace BlogFlow.API.Infrastructure
{
    public static class RateLimitingExtension
    {
        public static IServiceCollection AddAuthRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            const string REGISTER_POLICY = "register";
            const string LOGIN_POLICY = "login";
            const string REFRESH_POLICY = "refresh";
            const string REVOKE_POLICY = "revoke";

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

            services.AddRateLimiter(limiterOptions =>
            {
                var config = configuration
                    .GetSection(RateLimitOptions.SectionName)
                    .Get<RateLimitOptions>()!;

                CreateLimiter(limiterOptions, REGISTER_POLICY, config.Register);
                CreateLimiter(limiterOptions, LOGIN_POLICY, config.Login);
                CreateLimiter(limiterOptions, REFRESH_POLICY, config.Refresh);
                CreateLimiter(limiterOptions, REVOKE_POLICY, config.Revoke);

                limiterOptions.OnRejected = async (context, token) =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("RateLimiting");

                    logger.LogWarning(
                        "Rate limit exceeded for IP {IP} on path {Path}",
                        context.HttpContext.Connection.RemoteIpAddress?.ToString(),
                        context.HttpContext.Request.Path);

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = config.Rejected.Error,
                        message = config.Rejected.Message,
                        retryAfter = TimeSpan.FromSeconds(config.Rejected.WindowSecond)
                    }, cancellationToken: token);
                };
            });

            return services;
        }

        private static void CreateLimiter(
            RateLimiterOptions limiterOptions,
            string policy,
            RateLimitPolicyOptions endpointConfig)
        {
            limiterOptions.AddPolicy(policy, context =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = endpointConfig.MaxRequest,
                                Window = TimeSpan.FromSeconds(endpointConfig.WindowSecond),
                                QueueLimit = endpointConfig.QueueLimit,
                                AutoReplenishment = endpointConfig.AutoReplenishment
                            }));
        }
    }
}
