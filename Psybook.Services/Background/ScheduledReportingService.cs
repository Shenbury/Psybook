using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Psybook.Objects.Reporting;
using Psybook.Services.Reporting;
using System.Text.Json;

namespace Psybook.Services.Background
{
    /// <summary>
    /// Background service for processing scheduled reports
    /// </summary>
    public class ScheduledReportingService : BackgroundService
    {
        private readonly ILogger<ScheduledReportingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Guid, Services.Reporting.ScheduledReportRequest> _scheduledReports;
        private readonly Timer _timer;

        public ScheduledReportingService(
            ILogger<ScheduledReportingService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _scheduledReports = new Dictionary<Guid, Services.Reporting.ScheduledReportRequest>();
            
            // Check for scheduled reports every minute
            _timer = new Timer(CheckScheduledReports, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled Reporting Service started");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    
                    // Perform periodic cleanup and maintenance
                    await PerformMaintenance(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Scheduled Reporting Service is stopping due to cancellation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Scheduled Reporting Service");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled Reporting Service is stopping");
            
            _timer?.Dispose();
            
            await base.StopAsync(stoppingToken);
        }

        /// <summary>
        /// Adds a scheduled report
        /// </summary>
        public Guid AddScheduledReport(Services.Reporting.ScheduledReportRequest request)
        {
            var scheduleId = Guid.NewGuid();
            
            // Calculate next run date based on cron expression
            request.NextRunDate = CalculateNextRunDate(request.CronExpression);
            
            _scheduledReports[scheduleId] = request;
            
            _logger.LogInformation("Added scheduled report {Name} with ID {ScheduleId}, next run: {NextRun}", 
                request.Name, scheduleId, request.NextRunDate);
            
            return scheduleId;
        }

        /// <summary>
        /// Removes a scheduled report
        /// </summary>
        public bool RemoveScheduledReport(Guid scheduleId)
        {
            var removed = _scheduledReports.Remove(scheduleId);
            
            if (removed)
            {
                _logger.LogInformation("Removed scheduled report with ID {ScheduleId}", scheduleId);
            }
            
            return removed;
        }

        /// <summary>
        /// Gets all scheduled reports
        /// </summary>
        public List<(Guid Id, Services.Reporting.ScheduledReportRequest Request)> GetScheduledReports()
        {
            return _scheduledReports.Select(kvp => (kvp.Key, kvp.Value)).ToList();
        }

        private async void CheckScheduledReports(object? state)
        {
            try
            {
                var now = DateTime.UtcNow;
                var reportsToRun = _scheduledReports.Where(kvp => 
                    kvp.Value.IsActive && 
                    kvp.Value.NextRunDate.HasValue && 
                    kvp.Value.NextRunDate.Value <= now).ToList();

                foreach (var (scheduleId, reportRequest) in reportsToRun)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await ExecuteScheduledReport(scheduleId, reportRequest);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to execute scheduled report {ScheduleId}", scheduleId);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking scheduled reports");
            }
        }

        private async Task ExecuteScheduledReport(Guid scheduleId, Services.Reporting.ScheduledReportRequest reportRequest)
        {
            try
            {
                _logger.LogInformation("Executing scheduled report {Name} (ID: {ScheduleId})", 
                    reportRequest.Name, scheduleId);

                using var scope = _serviceProvider.CreateScope();
                var reportingService = scope.ServiceProvider.GetRequiredService<IReportingService>();
                
                // Generate the report
                var report = await reportingService.GenerateReportAsync(reportRequest.ReportParameters);
                
                // Send via email (simulated - would integrate with actual email service)
                await SendReportByEmail(report, reportRequest.EmailRecipients);
                
                // Update next run date
                _scheduledReports[scheduleId].NextRunDate = CalculateNextRunDate(reportRequest.CronExpression);
                
                _logger.LogInformation("Successfully executed scheduled report {Name}", reportRequest.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute scheduled report {Name} (ID: {ScheduleId})", 
                    reportRequest.Name, scheduleId);
            }
        }

        private async Task SendReportByEmail(ReportExport report, List<string> recipients)
        {
            // Simulated email sending - in production, integrate with SendGrid, SMTP, etc.
            _logger.LogInformation("Sending report {FileName} to {RecipientCount} recipients", 
                report.FileName, recipients.Count);

            foreach (var recipient in recipients)
            {
                _logger.LogInformation("Would send report to: {Recipient}", recipient);
            }

            // Simulate async email operation
            await Task.Delay(100);
        }

        private DateTime? CalculateNextRunDate(string cronExpression)
        {
            // Simplified cron parsing - in production, use a library like Cronos or Quartz
            // For demo purposes, support basic patterns:
            // "0 9 * * *" = daily at 9 AM
            // "0 9 * * 1" = weekly on Monday at 9 AM
            // "0 9 1 * *" = monthly on 1st at 9 AM

            try
            {
                var parts = cronExpression.Split(' ');
                if (parts.Length < 5)
                {
                    _logger.LogWarning("Invalid cron expression: {CronExpression}", cronExpression);
                    return DateTime.UtcNow.AddHours(1); // Default to 1 hour from now
                }

                var now = DateTime.UtcNow;
                var minute = int.Parse(parts[0]);
                var hour = int.Parse(parts[1]);

                // Daily pattern
                if (parts[2] == "*" && parts[3] == "*" && parts[4] == "*")
                {
                    var nextRun = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
                    if (nextRun <= now)
                    {
                        nextRun = nextRun.AddDays(1);
                    }
                    return nextRun;
                }

                // Weekly pattern (every Monday)
                if (parts[2] == "*" && parts[3] == "*" && parts[4] == "1")
                {
                    var daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
                    if (daysUntilMonday == 0) daysUntilMonday = 7; // If today is Monday, schedule for next Monday
                    
                    var nextRun = now.AddDays(daysUntilMonday).Date.AddHours(hour).AddMinutes(minute);
                    return nextRun;
                }

                // Monthly pattern (1st of month)
                if (parts[2] == "1" && parts[3] == "*" && parts[4] == "*")
                {
                    var nextRun = new DateTime(now.Year, now.Month, 1, hour, minute, 0);
                    if (nextRun <= now)
                    {
                        nextRun = nextRun.AddMonths(1);
                    }
                    return nextRun;
                }

                // Default: run in 1 hour
                return now.AddHours(1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing cron expression: {CronExpression}", cronExpression);
                return DateTime.UtcNow.AddHours(1);
            }
        }

        private async Task PerformMaintenance(CancellationToken cancellationToken)
        {
            try
            {
                // Clean up old report data, logs, etc.
                var activeReports = _scheduledReports.Count(kvp => kvp.Value.IsActive);
                
                _logger.LogDebug("Maintenance check: {ActiveReports} active scheduled reports", activeReports);
                
                // Remove inactive reports older than 30 days
                var cutoffDate = DateTime.UtcNow.AddDays(-30);
                var inactiveReports = _scheduledReports.Where(kvp => 
                    !kvp.Value.IsActive && 
                    (!kvp.Value.NextRunDate.HasValue || kvp.Value.NextRunDate.Value < cutoffDate))
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var reportId in inactiveReports)
                {
                    _scheduledReports.Remove(reportId);
                    _logger.LogInformation("Cleaned up inactive scheduled report: {ReportId}", reportId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during maintenance");
            }
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}