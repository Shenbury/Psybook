using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Psybook.Services.Monitoring;
using System.Diagnostics;

namespace Psybook.Services.Middleware
{
    /// <summary>
    /// Middleware for tracking API metrics and user activity
    /// </summary>
    public class MetricsTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISystemMetricsService _metricsService;

        public MetricsTrackingMiddleware(RequestDelegate next, ISystemMetricsService metricsService)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var endpoint = $"{context.Request.Method} {context.Request.Path}";

            try
            {
                // Record user activity if user is identified
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    var userId = context.User.Identity.Name ?? "anonymous";
                    _metricsService.RecordUserActivity(userId, endpoint);
                }
                else if (context.Request.Headers.ContainsKey("User-Agent"))
                {
                    // For anonymous users, use a hash of their IP + User Agent as identifier
                    var userIdentifier = GenerateAnonymousUserIdentifier(context);
                    _metricsService.RecordUserActivity(userIdentifier, endpoint);
                }

                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var responseTime = stopwatch.Elapsed.TotalMilliseconds;
                
                // Record API call metrics
                _metricsService.RecordApiCall(endpoint, responseTime);
            }
        }

        private static string GenerateAnonymousUserIdentifier(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var combined = $"{ip}_{userAgent}";
            
            // Create a simple hash for anonymous user tracking
            return Math.Abs(combined.GetHashCode()).ToString();
        }
    }

    /// <summary>
    /// Extension methods for registering metrics tracking middleware
    /// </summary>
    public static class MetricsTrackingMiddlewareExtensions
    {
        public static IApplicationBuilder UseMetricsTracking(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MetricsTrackingMiddleware>();
        }
    }
}