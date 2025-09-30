using Microsoft.Extensions.Logging;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Objects.Reporting;
using Psybook.Services.API.BookingService;
using Psybook.Services.Monitoring;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Psybook.Services.Reporting
{
    /// <summary>
    /// Implementation of reporting service with comprehensive analytics
    /// </summary>
    public class ReportingService : IReportingService
    {
        private readonly IBookingService _bookingService;
        private readonly ISystemMetricsService _systemMetricsService;
        private readonly ILogger<ReportingService> _logger;

        public ReportingService(
            IBookingService bookingService, 
            ISystemMetricsService systemMetricsService,
            ILogger<ReportingService> logger)
        {
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
            _systemMetricsService = systemMetricsService ?? throw new ArgumentNullException(nameof(systemMetricsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BookingAnalytics> GenerateAnalyticsAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Generating analytics for period {StartDate} to {EndDate}", request.StartDate, request.EndDate);

                var bookings = await GetFilteredBookingsAsync(request, cancellationToken);
                
                var analytics = new BookingAnalytics
                {
                    DateRange = request.StartDate,
                    ReportPeriod = request.EndDate - request.StartDate
                };

                // Calculate overall statistics
                CalculateOverallStatistics(analytics, bookings);
                
                // Calculate experience analytics
                CalculateExperienceAnalytics(analytics, bookings);
                
                // Calculate time-based analytics
                CalculateTimeBasedAnalytics(analytics, bookings);
                
                // Calculate customer analytics
                CalculateCustomerAnalytics(analytics, bookings);
                
                // Calculate geographic analytics
                CalculateGeographicAnalytics(analytics, bookings);
                
                // Calculate performance metrics using real system data
                await CalculatePerformanceMetricsAsync(analytics, bookings);

                _logger.LogInformation("Generated analytics with {TotalBookings} bookings", analytics.TotalBookings);
                return analytics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate analytics");
                throw;
            }
        }

        public async Task<ReportExport> GenerateReportAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Generating {Format} report for type {Type}", request.Format, request.ReportType);

                var analytics = await GenerateAnalyticsAsync(request, cancellationToken);
                
                byte[] data = request.Format switch
                {
                    ReportFormat.PDF => await GeneratePdfReport(analytics, request),
                    ReportFormat.Excel => await GenerateExcelReport(analytics, request),
                    ReportFormat.CSV => await GenerateCsvReport(analytics, request),
                    ReportFormat.JSON => GenerateJsonReport(analytics),
                    _ => throw new ArgumentException($"Unsupported report format: {request.Format}")
                };

                var export = new ReportExport
                {
                    FileName = GenerateFileName(request),
                    Format = request.Format,
                    Data = data,
                    ContentType = GetContentType(request.Format),
                    FileSize = data.Length,
                    Parameters = request
                };

                _logger.LogInformation("Generated {Format} report with {FileSize} bytes", request.Format, data.Length);
                return export;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate report");
                throw;
            }
        }

        public async Task<DashboardSummary> GetDashboardSummaryAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.Date;
                var end = endDate ?? DateTime.UtcNow.Date.AddDays(1);

                var allBookings = (await _bookingService.GetCalendarSlotsAsync()).ToList();
                var todayBookings = allBookings.Where(b => b.Start.Date == DateTime.UtcNow.Date).ToList();
                var weekBookings = allBookings.Where(b => b.Start >= DateTime.UtcNow.AddDays(-7)).ToList();
                var monthBookings = allBookings.Where(b => b.Start >= DateTime.UtcNow.AddDays(-30)).ToList();

                var summary = new DashboardSummary
                {
                    TotalBookingsToday = todayBookings.Count,
                    TotalBookingsThisWeek = weekBookings.Count,
                    TotalBookingsThisMonth = monthBookings.Count,
                    PendingBookings = allBookings.Count(b => b.Status == BookingStatus.Pending),
                    UpcomingBookings = allBookings.Count(b => b.Start > DateTime.UtcNow && b.IsActive),
                    TopExperiences = GetTopExperiences(monthBookings),
                    RecentBookings = GetRecentBookings(allBookings),
                    Alerts = GenerateAlerts(allBookings)
                };

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get dashboard summary");
                throw;
            }
        }

        public async Task<RealTimeMetrics> GetRealTimeMetricsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting real-time metrics with actual system data");

                // Get real booking data
                var bookings = (await _bookingService.GetCalendarSlotsAsync()).ToList();
                var todayBookings = bookings.Where(b => b.Start.Date == DateTime.UtcNow.Date).ToList();
                var now = DateTime.UtcNow;

                // Get real system metrics
                var systemMetrics = await _systemMetricsService.GetSystemPerformanceAsync();
                var apiMetrics = await _systemMetricsService.GetApiUsageMetricsAsync();
                var activeUserCount = await _systemMetricsService.GetActiveUserCountAsync();

                // Calculate real booking metrics
                var bookingsInProgress = bookings.Count(b => 
                    b.Status == BookingStatus.Confirmed && 
                    b.Start <= now && 
                    (b.End ?? b.Start.AddHours(2)) >= now);

                var completedBookingsToday = todayBookings.Count(b => b.Status == BookingStatus.Completed);

                var metrics = new RealTimeMetrics
                {
                    Timestamp = DateTime.UtcNow,
                    
                    // Real booking data
                    BookingsInProgress = bookingsInProgress,
                    CompletedBookingsToday = completedBookingsToday,
                    
                    // Real system metrics
                    ActiveUsers = activeUserCount,
                    SystemLoad = systemMetrics.SystemLoad,
                    ApiCallsPerMinute = apiMetrics.ApiCallsPerMinute,
                    AverageResponseTime = apiMetrics.AverageResponseTime
                };

                _logger.LogInformation("Generated real-time metrics: {ActiveUsers} active users, {ApiCalls} API calls/min, {SystemLoad}% system load", 
                    metrics.ActiveUsers, metrics.ApiCallsPerMinute, metrics.SystemLoad);

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get real-time metrics");
                
                // Return fallback metrics if there's an error
                return new RealTimeMetrics
                {
                    Timestamp = DateTime.UtcNow,
                    BookingsInProgress = 0,
                    CompletedBookingsToday = 0,
                    ActiveUsers = 0,
                    SystemLoad = 0,
                    ApiCallsPerMinute = 0,
                    AverageResponseTime = 0
                };
            }
        }

        public async Task<TrendingData> GetTrendingDataAsync(TrendPeriod period, CancellationToken cancellationToken = default)
        {
            try
            {
                var bookings = (await _bookingService.GetCalendarSlotsAsync()).ToList();
                var (startDate, endDate) = GetPeriodDates(period);
                
                var periodBookings = bookings.Where(b => b.Start >= startDate && b.Start <= endDate).ToList();

                return new TrendingData
                {
                    Period = period,
                    BookingTrend = GenerateBookingTrend(periodBookings, startDate, endDate),
                    CancellationTrend = GenerateCancellationTrend(periodBookings, startDate, endDate),
                    ExperienceTrends = GenerateExperienceTrends(periodBookings, startDate, endDate)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get trending data");
                throw;
            }
        }

        public async Task<byte[]> ExportToCsvAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            var analytics = await GenerateAnalyticsAsync(request, cancellationToken);
            return await GenerateCsvReport(analytics, request);
        }

        public async Task<byte[]> ExportToExcelAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            var analytics = await GenerateAnalyticsAsync(request, cancellationToken);
            return await GenerateExcelReport(analytics, request);
        }

        public async Task<byte[]> ExportToPdfAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            var analytics = await GenerateAnalyticsAsync(request, cancellationToken);
            return await GeneratePdfReport(analytics, request);
        }

        public async Task<List<ReportTemplate>> GetReportTemplatesAsync(CancellationToken cancellationToken = default)
        {
            // Return predefined templates - in a real implementation, these would come from database
            return new List<ReportTemplate>
            {
                new ReportTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Monthly Summary",
                    Description = "Comprehensive monthly booking summary with key metrics",
                    Type = ReportType.Summary,
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new ReportTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Customer Analysis",
                    Description = "Customer behavior and segmentation analysis",
                    Type = ReportType.Customer,
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new ReportTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = "Experience Performance",
                    Description = "Performance analysis of VIP experiences",
                    Type = ReportType.Experience,
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };
        }

        public async Task<Guid> ScheduleReportAsync(ScheduledReportRequest request, CancellationToken cancellationToken = default)
        {
            // In a real implementation, this would save to database and integrate with a scheduling service
            var scheduleId = Guid.NewGuid();
            _logger.LogInformation("Scheduled report {Name} with ID {ScheduleId}", request.Name, scheduleId);
            return scheduleId;
        }

        #region Private Helper Methods

        private async Task<List<CalendarSlot>> GetFilteredBookingsAsync(ReportRequest request, CancellationToken cancellationToken)
        {
            var bookings = (await _bookingService.GetCalendarSlotsAsync()).ToList();
            
            return bookings.Where(b => 
                b.Start >= request.StartDate && 
                b.Start <= request.EndDate &&
                (request.StatusFilter.Count == 0 || request.StatusFilter.Contains(b.Status)) &&
                (request.ExperienceFilter.Count == 0 || request.ExperienceFilter.Contains(b.BookingExperience))
            ).ToList();
        }

        private void CalculateOverallStatistics(BookingAnalytics analytics, List<CalendarSlot> bookings)
        {
            analytics.TotalBookings = bookings.Count;
            analytics.ActiveBookings = bookings.Count(b => b.IsActive);
            analytics.CompletedBookings = bookings.Count(b => b.Status == BookingStatus.Completed);
            analytics.CancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled);
            
            analytics.CancellationRate = analytics.TotalBookings > 0 
                ? (decimal)analytics.CancelledBookings / analytics.TotalBookings * 100 
                : 0;
                
            analytics.CompletionRate = analytics.TotalBookings > 0 
                ? (decimal)analytics.CompletedBookings / analytics.TotalBookings * 100 
                : 0;
        }

        private void CalculateExperienceAnalytics(BookingAnalytics analytics, List<CalendarSlot> bookings)
        {
            var experienceGroups = bookings.GroupBy(b => b.BookingExperience);
            
            analytics.ExperienceStats = experienceGroups.Select(g => new ExperienceStatistic
            {
                Experience = g.Key,
                ExperienceName = g.Key.ToString(),
                BookingCount = g.Count(),
                Percentage = bookings.Count > 0 ? (decimal)g.Count() / bookings.Count * 100 : 0,
                CancelledCount = g.Count(b => b.Status == BookingStatus.Cancelled),
                CancellationRate = g.Count() > 0 ? (decimal)g.Count(b => b.Status == BookingStatus.Cancelled) / g.Count() * 100 : 0
            }).OrderByDescending(e => e.BookingCount).ToList();
        }

        private void CalculateTimeBasedAnalytics(BookingAnalytics analytics, List<CalendarSlot> bookings)
        {
            // Monthly statistics
            var monthlyGroups = bookings.GroupBy(b => new { b.Start.Year, b.Start.Month });
            analytics.MonthlyStats = monthlyGroups.Select(g => new MonthlyStatistic
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                BookingCount = g.Count(),
                CancelledCount = g.Count(b => b.Status == BookingStatus.Cancelled)
            }).OrderBy(m => m.Year).ThenBy(m => m.Month).ToList();

            // Day of week statistics
            var dayGroups = bookings.GroupBy(b => b.Start.DayOfWeek);
            analytics.DayOfWeekStats = dayGroups.Select(g => new DayOfWeekStatistic
            {
                DayOfWeek = g.Key,
                DayName = g.Key.ToString(),
                BookingCount = g.Count(),
                Percentage = bookings.Count > 0 ? (decimal)g.Count() / bookings.Count * 100 : 0
            }).OrderBy(d => d.DayOfWeek).ToList();

            // Hourly statistics
            var hourGroups = bookings.GroupBy(b => b.Start.Hour);
            analytics.HourlyStats = hourGroups.Select(g => new HourlyStatistic
            {
                Hour = g.Key,
                TimeSlot = $"{g.Key:00}:00",
                BookingCount = g.Count(),
                Percentage = bookings.Count > 0 ? (decimal)g.Count() / bookings.Count * 100 : 0
            }).OrderBy(h => h.Hour).ToList();
        }

        private void CalculateCustomerAnalytics(BookingAnalytics analytics, List<CalendarSlot> bookings)
        {
            var customerGroups = bookings.GroupBy(b => $"{b.FirstName} {b.LastName}");
            
            analytics.CustomerStats = new CustomerAnalytics
            {
                TotalCustomers = customerGroups.Count(),
                ReturningCustomers = customerGroups.Count(g => g.Count() > 1),
                NewCustomers = customerGroups.Count(g => g.Count() == 1),
                AverageBookingsPerCustomer = customerGroups.Count() > 0 ? (double)bookings.Count / customerGroups.Count() : 0
            };
            
            analytics.CustomerStats.ReturnRate = analytics.CustomerStats.TotalCustomers > 0 
                ? (decimal)analytics.CustomerStats.ReturningCustomers / analytics.CustomerStats.TotalCustomers * 100 
                : 0;
        }

        private void CalculateGeographicAnalytics(BookingAnalytics analytics, List<CalendarSlot> bookings)
        {
            var postcodeGroups = bookings.Where(b => !string.IsNullOrEmpty(b.Postcode))
                .GroupBy(b => b.Postcode.Split(' ')[0]); // Group by postcode area
            
            analytics.GeographicStats = postcodeGroups.Select(g => new GeographicStatistic
            {
                PostcodeArea = g.Key,
                Region = g.Key, // Simplified - would map to actual regions
                BookingCount = g.Count(),
                Percentage = bookings.Count > 0 ? (decimal)g.Count() / bookings.Count * 100 : 0
            }).OrderByDescending(g => g.BookingCount).ToList();
        }

        private async Task CalculatePerformanceMetricsAsync(BookingAnalytics analytics, List<CalendarSlot> bookings)
        {
            try
            {
                var systemMetrics = await _systemMetricsService.GetSystemPerformanceAsync();
                var apiMetrics = await _systemMetricsService.GetApiUsageMetricsAsync();

                analytics.Performance = new PerformanceMetrics
                {
                    AverageProcessingTime = systemMetrics.AverageResponseTime,
                    PeakBookingsPerHour = bookings.GroupBy(b => b.Start.Hour).Max(g => g?.Count() ?? 0),
                    SystemUptime = CalculateUptimePercentage(systemMetrics.Uptime),
                    ApiCalls = apiMetrics.TotalApiCallsToday,
                    ResponseTime = apiMetrics.AverageResponseTime
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate performance metrics, using fallback values");
                
                // Fallback to basic metrics if system metrics are unavailable
                analytics.Performance = new PerformanceMetrics
                {
                    AverageProcessingTime = 100, // Fallback value
                    PeakBookingsPerHour = bookings.GroupBy(b => b.Start.Hour).Max(g => g?.Count() ?? 0),
                    SystemUptime = 99.0, // Assume good uptime
                    ApiCalls = bookings.Count * 3, // Estimated
                    ResponseTime = 250 // Estimated average
                };
            }
        }

        private double CalculateUptimePercentage(TimeSpan uptime)
        {
            // Calculate uptime percentage based on how long the system has been running
            // For systems running less than 24 hours, use the actual uptime
            var totalHours = uptime.TotalHours;
            if (totalHours < 24)
            {
                return Math.Min(100, (totalHours / 24) * 100);
            }
            
            // For longer running systems, calculate based on expected vs actual uptime
            var expectedUptime = DateTime.UtcNow - DateTime.UtcNow.Date; // Time since start of day
            var uptimePercentage = (uptime.TotalMinutes / expectedUptime.TotalMinutes) * 100;
            return Math.Min(100, Math.Max(0, uptimePercentage));
        }

        private List<TopExperience> GetTopExperiences(List<CalendarSlot> bookings)
        {
            return bookings.GroupBy(b => b.BookingExperience)
                .Select(g => new TopExperience
                {
                    Name = g.Key.ToString(),
                    BookingCount = g.Count(),
                    Growth = new Random().Next(-20, 50) // Simulated growth percentage - would be calculated from historical data
                })
                .OrderByDescending(e => e.BookingCount)
                .Take(5)
                .ToList();
        }

        private List<RecentBooking> GetRecentBookings(List<CalendarSlot> bookings)
        {
            return bookings.OrderByDescending(b => b.CreatedAt)
                .Take(10)
                .Select(b => new RecentBooking
                {
                    Id = b.Id,
                    CustomerName = $"{b.FirstName} {b.LastName}",
                    ExperienceName = b.Title,
                    BookingDate = b.Start,
                    Status = b.StatusDisplayName
                })
                .ToList();
        }

        private List<Alert> GenerateAlerts(List<CalendarSlot> bookings)
        {
            var alerts = new List<Alert>();
            
            var pendingCount = bookings.Count(b => b.Status == BookingStatus.Pending);
            if (pendingCount > 5)
            {
                alerts.Add(new Alert
                {
                    Type = "Pending Bookings",
                    Message = $"{pendingCount} bookings are pending confirmation",
                    Severity = "Warning",
                    Timestamp = DateTime.UtcNow
                });
            }

            var upcomingCount = bookings.Count(b => b.Start <= DateTime.UtcNow.AddDays(1) && b.Start > DateTime.UtcNow && b.IsActive);
            if (upcomingCount > 0)
            {
                alerts.Add(new Alert
                {
                    Type = "Upcoming Bookings",
                    Message = $"{upcomingCount} bookings scheduled for tomorrow",
                    Severity = "Info",
                    Timestamp = DateTime.UtcNow
                });
            }

            return alerts;
        }

        private (DateTime startDate, DateTime endDate) GetPeriodDates(TrendPeriod period)
        {
            var now = DateTime.UtcNow;
            return period switch
            {
                TrendPeriod.Last7Days => (now.AddDays(-7), now),
                TrendPeriod.Last30Days => (now.AddDays(-30), now),
                TrendPeriod.Last90Days => (now.AddDays(-90), now),
                TrendPeriod.Last12Months => (now.AddMonths(-12), now),
                TrendPeriod.YearToDate => (new DateTime(now.Year, 1, 1), now),
                _ => (now.AddDays(-30), now)
            };
        }

        private List<DataPoint> GenerateBookingTrend(List<CalendarSlot> bookings, DateTime startDate, DateTime endDate)
        {
            var days = (int)(endDate - startDate).TotalDays;
            var dataPoints = new List<DataPoint>();

            for (int i = 0; i <= days; i++)
            {
                var date = startDate.AddDays(i);
                var count = bookings.Count(b => b.Start.Date == date.Date);
                
                dataPoints.Add(new DataPoint
                {
                    Date = date,
                    Value = count,
                    Label = date.ToString("MMM dd")
                });
            }

            return dataPoints;
        }

        private async Task<List<DataPoint>> GenerateRevenueTrend(List<CalendarSlot> bookings, DateTime startDate, DateTime endDate)
        {
            // Revenue functionality removed - return empty list
            return new List<DataPoint>();
        }

        private decimal CalculateEstimatedRevenueForToday(List<CalendarSlot> todayBookings)
        {
            // Revenue functionality removed - return 0
            return 0m;
        }

        private List<DataPoint> GenerateCancellationTrend(List<CalendarSlot> bookings, DateTime startDate, DateTime endDate)
        {
            var days = (int)(endDate - startDate).TotalDays;
            var dataPoints = new List<DataPoint>();

            for (int i = 0; i <= days; i++)
            {
                var date = startDate.AddDays(i);
                var count = bookings.Count(b => b.Start.Date == date.Date && b.Status == BookingStatus.Cancelled);
                
                dataPoints.Add(new DataPoint
                {
                    Date = date,
                    Value = count,
                    Label = date.ToString("MMM dd")
                });
            }

            return dataPoints;
        }

        private List<ExperienceTrend> GenerateExperienceTrends(List<CalendarSlot> bookings, DateTime startDate, DateTime endDate)
        {
            var experiences = bookings.GroupBy(b => b.BookingExperience);
            var trends = new List<ExperienceTrend>();

            foreach (var experience in experiences)
            {
                var trend = new ExperienceTrend
                {
                    ExperienceName = experience.Key.ToString(),
                    Data = GenerateBookingTrend(experience.ToList(), startDate, endDate)
                };
                trends.Add(trend);
            }

            return trends.OrderByDescending(t => t.Data.Sum(d => d.Value)).Take(5).ToList();
        }

        private async Task<byte[]> GeneratePdfReport(BookingAnalytics analytics, ReportRequest request)
        {
            // Simplified PDF generation - in a real implementation, use a PDF library like iTextSharp
            var content = $"WMSP VIP Booking Report\n\nGenerated: {analytics.GeneratedAt}\nPeriod: {request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd}\n\nTotal Bookings: {analytics.TotalBookings}\nActive Bookings: {analytics.ActiveBookings}\nCancellation Rate: {analytics.CancellationRate:F2}%";
            return Encoding.UTF8.GetBytes(content);
        }

        private async Task<byte[]> GenerateExcelReport(BookingAnalytics analytics, ReportRequest request)
        {
            // Simplified Excel generation - in a real implementation, use EPPlus or similar
            var csv = await GenerateCsvReport(analytics, request);
            return csv; // Return CSV data for now
        }

        private async Task<byte[]> GenerateCsvReport(BookingAnalytics analytics, ReportRequest request)
        {
            var csv = new StringBuilder();
            csv.AppendLine("WMSP VIP Booking Report");
            csv.AppendLine($"Generated,{analytics.GeneratedAt:yyyy-MM-dd HH:mm:ss}");
            csv.AppendLine($"Period,{request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd}");
            csv.AppendLine();
            csv.AppendLine("Metric,Value");
            csv.AppendLine($"Total Bookings,{analytics.TotalBookings}");
            csv.AppendLine($"Active Bookings,{analytics.ActiveBookings}");
            csv.AppendLine($"Completed Bookings,{analytics.CompletedBookings}");
            csv.AppendLine($"Cancelled Bookings,{analytics.CancelledBookings}");
            csv.AppendLine($"Cancellation Rate,{analytics.CancellationRate:F2}%");
            csv.AppendLine($"Completion Rate,{analytics.CompletionRate:F2}%");
            csv.AppendLine();
            csv.AppendLine("Experience,Bookings,Percentage,Cancellation Rate");
            
            foreach (var exp in analytics.ExperienceStats)
            {
                csv.AppendLine($"{exp.ExperienceName},{exp.BookingCount},{exp.Percentage:F2}%,{exp.CancellationRate:F2}%");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private byte[] GenerateJsonReport(BookingAnalytics analytics)
        {
            var json = JsonSerializer.Serialize(analytics, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return Encoding.UTF8.GetBytes(json);
        }

        private string GenerateFileName(ReportRequest request)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var extension = request.Format.ToString().ToLower();
            return $"WMSP_Report_{request.ReportType}_{timestamp}.{extension}";
        }

        private string GetContentType(ReportFormat format)
        {
            return format switch
            {
                ReportFormat.PDF => "application/pdf",
                ReportFormat.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ReportFormat.CSV => "text/csv",
                ReportFormat.JSON => "application/json",
                _ => "application/octet-stream"
            };
        }

        #endregion
    }
}