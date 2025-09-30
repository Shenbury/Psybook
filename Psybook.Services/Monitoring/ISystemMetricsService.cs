using Psybook.Objects.Reporting;

namespace Psybook.Services.Monitoring
{
    /// <summary>
    /// Service for collecting real-time system metrics
    /// </summary>
    public interface ISystemMetricsService
    {
        /// <summary>
        /// Gets current system performance metrics
        /// </summary>
        Task<SystemPerformanceMetrics> GetSystemPerformanceAsync();

        /// <summary>
        /// Gets current API usage metrics
        /// </summary>
        Task<ApiUsageMetrics> GetApiUsageMetricsAsync();

        /// <summary>
        /// Gets active user count (estimated)
        /// </summary>
        Task<int> GetActiveUserCountAsync();

        /// <summary>
        /// Records an API call for metrics tracking
        /// </summary>
        void RecordApiCall(string endpoint, double responseTime);

        /// <summary>
        /// Records a user activity
        /// </summary>
        void RecordUserActivity(string userId, string activity);
    }

    /// <summary>
    /// System performance metrics
    /// </summary>
    public class SystemPerformanceMetrics
    {
        public double CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
        public double SystemLoad { get; set; }
        public double AverageResponseTime { get; set; }
        public TimeSpan Uptime { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// API usage metrics
    /// </summary>
    public class ApiUsageMetrics
    {
        public int ApiCallsPerMinute { get; set; }
        public int TotalApiCallsToday { get; set; }
        public double AverageResponseTime { get; set; }
        public Dictionary<string, int> EndpointCalls { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}