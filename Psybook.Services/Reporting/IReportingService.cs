using Psybook.Objects.DbModels;
using Psybook.Objects.Reporting;

namespace Psybook.Services.Reporting
{
    /// <summary>
    /// Service interface for generating reports and analytics
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        /// Generates comprehensive booking analytics
        /// </summary>
        Task<BookingAnalytics> GenerateAnalyticsAsync(ReportRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Generates a report export in the specified format
        /// </summary>
        Task<ReportExport> GenerateReportAsync(ReportRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets dashboard summary data
        /// </summary>
        Task<DashboardSummary> GetDashboardSummaryAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets real-time metrics for monitoring
        /// </summary>
        Task<RealTimeMetrics> GetRealTimeMetricsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets trending data for charts
        /// </summary>
        Task<TrendingData> GetTrendingDataAsync(TrendPeriod period, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exports booking data to CSV
        /// </summary>
        Task<byte[]> ExportToCsvAsync(ReportRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exports booking data to Excel
        /// </summary>
        Task<byte[]> ExportToExcelAsync(ReportRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exports booking data to PDF
        /// </summary>
        Task<byte[]> ExportToPdfAsync(ReportRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets available report templates
        /// </summary>
        Task<List<ReportTemplate>> GetReportTemplatesAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Schedules a report to be generated and emailed
        /// </summary>
        Task<Guid> ScheduleReportAsync(ScheduledReportRequest request, CancellationToken cancellationToken = default);
    }
    
    /// <summary>
    /// Dashboard summary data
    /// </summary>
    public class DashboardSummary
    {
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public int TotalBookingsToday { get; set; }
        public int TotalBookingsThisWeek { get; set; }
        public int TotalBookingsThisMonth { get; set; }
        public decimal RevenueMTD { get; set; }
        public decimal RevenueYTD { get; set; }
        public int PendingBookings { get; set; }
        public int UpcomingBookings { get; set; }
        public List<TopExperience> TopExperiences { get; set; } = new();
        public List<RecentBooking> RecentBookings { get; set; } = new();
        public List<Alert> Alerts { get; set; } = new();
    }
    
    public class TopExperience
    {
        public string Name { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal Growth { get; set; }
    }
    
    public class RecentBooking
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ExperienceName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
    
    public class Alert
    {
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
    
    /// <summary>
    /// Real-time metrics for live dashboard
    /// </summary>
    public class RealTimeMetrics
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int ActiveUsers { get; set; }
        public int BookingsInProgress { get; set; }
        public int CompletedBookingsToday { get; set; }
        public decimal RevenueToday { get; set; }
        public double SystemLoad { get; set; }
        public int ApiCallsPerMinute { get; set; }
        public double AverageResponseTime { get; set; }
    }
    
    /// <summary>
    /// Trending data for charts and graphs
    /// </summary>
    public class TrendingData
    {
        public TrendPeriod Period { get; set; }
        public List<DataPoint> BookingTrend { get; set; } = new();
        public List<DataPoint> RevenueTrend { get; set; } = new();
        public List<DataPoint> CancellationTrend { get; set; } = new();
        public List<ExperienceTrend> ExperienceTrends { get; set; } = new();
    }
    
    public class DataPoint
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Label { get; set; } = string.Empty;
    }
    
    public class ExperienceTrend
    {
        public string ExperienceName { get; set; } = string.Empty;
        public List<DataPoint> Data { get; set; } = new();
    }
    
    public enum TrendPeriod
    {
        Last7Days,
        Last30Days,
        Last90Days,
        Last12Months,
        YearToDate,
        Custom
    }
    
    /// <summary>
    /// Report template definition
    /// </summary>
    public class ReportTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ReportType Type { get; set; }
        public string TemplateData { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Scheduled report request
    /// </summary>
    public class ScheduledReportRequest
    {
        public string Name { get; set; } = string.Empty;
        public ReportRequest ReportParameters { get; set; } = new();
        public string CronExpression { get; set; } = string.Empty;
        public List<string> EmailRecipients { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public DateTime? NextRunDate { get; set; }
    }
}