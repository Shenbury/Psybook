using Psybook.Objects.Enums;

namespace Psybook.Objects.Reporting
{
    /// <summary>
    /// Comprehensive analytics data for booking reports
    /// </summary>
    public class BookingAnalytics
    {
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public DateTimeOffset DateRange { get; set; }
        public TimeSpan ReportPeriod { get; set; }
        
        // Overall Statistics
        public int TotalBookings { get; set; }
        public int ActiveBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal CancellationRate { get; set; }
        public decimal CompletionRate { get; set; }
        
        // Revenue Analytics
        public decimal EstimatedRevenue { get; set; }
        public decimal AverageBookingValue { get; set; }
        public decimal RevenueGrowth { get; set; }
        
        // Experience Analytics
        public List<ExperienceStatistic> ExperienceStats { get; set; } = new();
        
        // Time-based Analytics
        public List<MonthlyStatistic> MonthlyStats { get; set; } = new();
        public List<DayOfWeekStatistic> DayOfWeekStats { get; set; } = new();
        public List<HourlyStatistic> HourlyStats { get; set; } = new();
        
        // Customer Analytics
        public CustomerAnalytics CustomerStats { get; set; } = new();
        
        // Geographic Analytics
        public List<GeographicStatistic> GeographicStats { get; set; } = new();
        
        // Performance Metrics
        public PerformanceMetrics Performance { get; set; } = new();
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
    
    /// <summary>
    /// Data point for trending charts
    /// </summary>
    public class DataPoint
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
        public DateTime Date { get; set; }
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
    
    public class ExperienceStatistic
    {
        public BookingExperience Experience { get; set; }
        public string ExperienceName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal Percentage { get; set; }
        public decimal Revenue { get; set; }
        public int CancelledCount { get; set; }
        public decimal CancellationRate { get; set; }
        public double AverageRating { get; set; }
        public List<MonthlyTrend> Trends { get; set; } = new();
    }
    
    public class MonthlyStatistic
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public int CancelledCount { get; set; }
        public decimal GrowthRate { get; set; }
    }
    
    public class DayOfWeekStatistic
    {
        public DayOfWeek DayOfWeek { get; set; }
        public string DayName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal AverageBookings { get; set; }
        public decimal Percentage { get; set; }
    }
    
    public class HourlyStatistic
    {
        public int Hour { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal Percentage { get; set; }
    }
    
    public class CustomerAnalytics
    {
        public int TotalCustomers { get; set; }
        public int ReturningCustomers { get; set; }
        public int NewCustomers { get; set; }
        public decimal ReturnRate { get; set; }
        public double AverageBookingsPerCustomer { get; set; }
        public List<CustomerSegment> Segments { get; set; } = new();
    }
    
    public class CustomerSegment
    {
        public string SegmentName { get; set; } = string.Empty;
        public int CustomerCount { get; set; }
        public decimal Percentage { get; set; }
        public decimal AverageValue { get; set; }
    }
    
    public class GeographicStatistic
    {
        public string Region { get; set; } = string.Empty;
        public string PostcodeArea { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal Percentage { get; set; }
        public decimal Revenue { get; set; }
    }
    
    public class PerformanceMetrics
    {
        public double AverageProcessingTime { get; set; }
        public int PeakBookingsPerHour { get; set; }
        public double SystemUptime { get; set; }
        public int ApiCalls { get; set; }
        public double ResponseTime { get; set; }
    }
    
    public class MonthlyTrend
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Value { get; set; }
        public decimal Change { get; set; }
    }
    
    /// <summary>
    /// Report generation request parameters
    /// </summary>
    public class ReportRequest
    {
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddMonths(-1);
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public List<BookingStatus> StatusFilter { get; set; } = new();
        public List<BookingExperience> ExperienceFilter { get; set; } = new();
        public ReportType ReportType { get; set; } = ReportType.Summary;
        public ReportFormat Format { get; set; } = ReportFormat.Dashboard;
        public bool IncludeCustomerDetails { get; set; } = false;
        public bool IncludeFinancialData { get; set; } = true;
        public string? CustomFilters { get; set; }
    }
    
    public enum ReportType
    {
        Summary,
        Detailed,
        Financial,
        Customer,
        Experience,
        Geographic,
        Performance
    }
    
    public enum ReportFormat
    {
        Dashboard,
        PDF,
        Excel,
        CSV,
        JSON
    }
    
    /// <summary>
    /// Export data for various report formats
    /// </summary>
    public class ReportExport
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = string.Empty;
        public ReportFormat Format { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string GeneratedBy { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public ReportRequest Parameters { get; set; } = new();
    }
}