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
}