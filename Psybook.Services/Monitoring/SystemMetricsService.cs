using Microsoft.Extensions.Logging;
using Psybook.Objects.Reporting;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Psybook.Services.Monitoring
{
    /// <summary>
    /// Implementation of system metrics service using real system data
    /// </summary>
    public class SystemMetricsService : ISystemMetricsService
    {
        private readonly ILogger<SystemMetricsService> _logger;
        private readonly Process _currentProcess;
        private readonly DateTime _startTime;
        
        // Thread-safe collections for tracking metrics
        private readonly ConcurrentQueue<ApiCallRecord> _recentApiCalls;
        private readonly ConcurrentDictionary<string, DateTime> _activeUsers;
        private readonly ConcurrentDictionary<string, int> _endpointCalls;
        
        // For CPU usage calculation
        private DateTime _lastCpuTime;
        private TimeSpan _lastTotalProcessorTime;

        public SystemMetricsService(ILogger<SystemMetricsService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentProcess = Process.GetCurrentProcess();
            _startTime = DateTime.UtcNow;
            _recentApiCalls = new ConcurrentQueue<ApiCallRecord>();
            _activeUsers = new ConcurrentDictionary<string, DateTime>();
            _endpointCalls = new ConcurrentDictionary<string, int>();
            
            // Initialize CPU usage tracking
            _lastCpuTime = DateTime.UtcNow;
            _lastTotalProcessorTime = _currentProcess.TotalProcessorTime;
        }

        public async Task<SystemPerformanceMetrics> GetSystemPerformanceAsync()
        {
            try
            {
                var metrics = new SystemPerformanceMetrics
                {
                    Uptime = DateTime.UtcNow - _startTime,
                    LastUpdated = DateTime.UtcNow
                };

                // Get CPU usage using cross-platform method
                metrics.CpuUsage = GetProcessCpuUsage();

                // Get memory usage using cross-platform method
                metrics.MemoryUsage = GetProcessMemoryUsage();

                // Calculate system load (combination of CPU and memory)
                metrics.SystemLoad = (metrics.CpuUsage + metrics.MemoryUsage) / 2;

                // Calculate average response time from recent API calls
                metrics.AverageResponseTime = CalculateAverageResponseTime();

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get system performance metrics");
                throw;
            }
        }

        public async Task<ApiUsageMetrics> GetApiUsageMetricsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var oneMinuteAgo = now.AddMinutes(-1);
                var startOfDay = now.Date;

                // Count API calls in the last minute
                var callsLastMinute = _recentApiCalls.Count(call => call.Timestamp >= oneMinuteAgo);
                
                // Count total API calls today
                var callsToday = _recentApiCalls.Count(call => call.Timestamp >= startOfDay);

                // Calculate average response time
                var averageResponseTime = CalculateAverageResponseTime();

                return new ApiUsageMetrics
                {
                    ApiCallsPerMinute = callsLastMinute,
                    TotalApiCallsToday = callsToday,
                    AverageResponseTime = averageResponseTime,
                    EndpointCalls = new Dictionary<string, int>(_endpointCalls),
                    LastUpdated = now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get API usage metrics");
                throw;
            }
        }

        public async Task<int> GetActiveUserCountAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var activeThreshold = now.AddMinutes(-15); // Consider users active if they had activity in last 15 minutes

                // Clean up old user activities
                var inactiveUsers = _activeUsers
                    .Where(kvp => kvp.Value < activeThreshold)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var inactiveUser in inactiveUsers)
                {
                    _activeUsers.TryRemove(inactiveUser, out _);
                }

                return _activeUsers.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get active user count");
                return 0;
            }
        }

        public void RecordApiCall(string endpoint, double responseTime)
        {
            try
            {
                var record = new ApiCallRecord
                {
                    Endpoint = endpoint,
                    ResponseTime = responseTime,
                    Timestamp = DateTime.UtcNow
                };

                _recentApiCalls.Enqueue(record);
                _endpointCalls.AddOrUpdate(endpoint, 1, (key, value) => value + 1);

                // Keep only recent calls (last 24 hours)
                var cutoff = DateTime.UtcNow.AddHours(-24);
                while (_recentApiCalls.TryPeek(out var oldestCall) && oldestCall.Timestamp < cutoff)
                {
                    _recentApiCalls.TryDequeue(out _);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record API call for endpoint {Endpoint}", endpoint);
            }
        }

        public void RecordUserActivity(string userId, string activity)
        {
            try
            {
                _activeUsers.AddOrUpdate(userId, DateTime.UtcNow, (key, value) => DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record user activity for user {UserId}", userId);
            }
        }

        private double GetProcessCpuUsage()
        {
            try
            {
                var now = DateTime.UtcNow;
                var nowTotalProcessorTime = _currentProcess.TotalProcessorTime;
                
                var timeDiff = now - _lastCpuTime;
                var processorTimeDiff = nowTotalProcessorTime - _lastTotalProcessorTime;
                
                // Update for next calculation
                _lastCpuTime = now;
                _lastTotalProcessorTime = nowTotalProcessorTime;
                
                // Avoid division by zero
                if (timeDiff.TotalMilliseconds == 0)
                    return 0;
                
                var cpuUsage = (processorTimeDiff.TotalMilliseconds / timeDiff.TotalMilliseconds) / Environment.ProcessorCount * 100;
                return Math.Min(100, Math.Max(0, cpuUsage));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to calculate process CPU usage");
                return 0;
            }
        }

        private double GetProcessMemoryUsage()
        {
            try
            {
                // Get current process memory usage
                var workingSet = _currentProcess.WorkingSet64;
                var totalMemory = GC.GetTotalMemory(false);
                
                // Get system memory info (cross-platform approach)
                var gcMemoryInfo = GC.GetGCMemoryInfo();
                var totalAvailableMemory = gcMemoryInfo.TotalAvailableMemoryBytes;
                
                if (totalAvailableMemory > 0)
                {
                    return Math.Min(100, Math.Max(0, (double)workingSet / totalAvailableMemory * 100));
                }
                
                // Fallback calculation if system memory info is not available
                var estimatedSystemMemory = Math.Max(workingSet * 10, totalMemory * 5);
                return Math.Min(100, Math.Max(0, (double)workingSet / estimatedSystemMemory * 100));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to calculate process memory usage");
                return 0;
            }
        }

        private double CalculateAverageResponseTime()
        {
            try
            {
                var recentCalls = _recentApiCalls.ToArray();
                if (recentCalls.Length == 0)
                    return 0;

                var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);
                var recentCallsLastMinute = recentCalls.Where(call => call.Timestamp >= oneMinuteAgo).ToArray();
                
                if (recentCallsLastMinute.Length == 0)
                    return 0;

                return recentCallsLastMinute.Average(call => call.ResponseTime);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to calculate average response time");
                return 0;
            }
        }

        public void Dispose()
        {
            try
            {
                _currentProcess?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error disposing SystemMetricsService");
            }
        }

        private class ApiCallRecord
        {
            public string Endpoint { get; set; } = string.Empty;
            public double ResponseTime { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}