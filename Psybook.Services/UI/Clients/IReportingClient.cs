using Psybook.Objects.Reporting;

namespace Psybook.Services.UI.Clients
{
    /// <summary>
    /// HTTP client interface for reporting-related API operations.
    /// Handles communication with the reporting API for analytics and dashboard data.
    /// </summary>
    public interface IReportingClient
    {
        /// <summary>
        /// Gets dashboard summary data from the API
        /// </summary>
        /// <param name="startDate">Optional start date for the summary period</param>
        /// <param name="endDate">Optional end date for the summary period</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dashboard summary data</returns>
        Task<DashboardSummary> GetDashboardSummaryAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets comprehensive booking analytics from the API
        /// </summary>
        /// <param name="request">Report request parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Booking analytics data</returns>
        Task<BookingAnalytics> GetAnalyticsAsync(ReportRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets trending data for charts from the API
        /// </summary>
        /// <param name="period">Trend period to retrieve</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Trending data</returns>
        Task<TrendingData> GetTrendingDataAsync(TrendPeriod period = TrendPeriod.Last30Days, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates and downloads a report from the API
        /// </summary>
        /// <param name="request">Report request parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Report export data</returns>
        Task<ReportExport> GenerateReportAsync(ReportRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets real-time metrics for monitoring from the API
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Real-time metrics</returns>
        Task<RealTimeMetrics> GetRealTimeMetricsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets available report templates from the API
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of available report templates</returns>
        Task<List<ReportTemplate>> GetReportTemplatesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Schedules a report to be generated and emailed via the API
        /// </summary>
        /// <param name="request">Scheduled report request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Schedule ID</returns>
        Task<Guid> ScheduleReportAsync(ScheduledReportRequest request, CancellationToken cancellationToken = default);
    }
}