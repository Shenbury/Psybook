using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Psybook.Objects.Reporting;
using Psybook.Services.Reporting;

namespace Psybook.API.Controllers
{
    /// <summary>
    /// API controller for reporting and analytics operations
    /// </summary>
    [ApiController]
    [AllowAnonymous] // Temporarily allow anonymous access for development
    [Route("api/[controller]")]
    public class ReportingController : ControllerBase
    {
        private readonly IReportingService _reportingService;
        private readonly ILogger<ReportingController> _logger;

        public ReportingController(IReportingService reportingService, ILogger<ReportingController> logger)
        {
            _reportingService = reportingService ?? throw new ArgumentNullException(nameof(reportingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets dashboard summary data
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Getting dashboard summary for period {StartDate} to {EndDate}", startDate, endDate);
                
                var summary = await _reportingService.GetDashboardSummaryAsync(startDate, endDate);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get dashboard summary");
                return StatusCode(500, "Failed to retrieve dashboard summary");
            }
        }

        /// <summary>
        /// Gets comprehensive booking analytics
        /// </summary>
        [HttpPost("analytics")]
        public async Task<IActionResult> GetAnalytics([FromBody] ReportRequest request)
        {
            try
            {
                _logger.LogInformation("Generating analytics for {ReportType} from {StartDate} to {EndDate}", 
                    request.ReportType, request.StartDate, request.EndDate);
                
                var analytics = await _reportingService.GenerateAnalyticsAsync(request);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate analytics");
                return StatusCode(500, "Failed to generate analytics");
            }
        }

        /// <summary>
        /// Generates and downloads a report
        /// </summary>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport([FromBody] ReportRequest request)
        {
            try
            {
                _logger.LogInformation("Generating {Format} report for {ReportType}", request.Format, request.ReportType);
                
                var report = await _reportingService.GenerateReportAsync(request);
                
                return File(report.Data, report.ContentType, report.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate report");
                return StatusCode(500, "Failed to generate report");
            }
        }

        /// <summary>
        /// Gets real-time metrics for live dashboard
        /// </summary>
        [HttpGet("realtime-metrics")]
        public async Task<IActionResult> GetRealTimeMetrics()
        {
            try
            {
                var metrics = await _reportingService.GetRealTimeMetricsAsync();
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get real-time metrics");
                return StatusCode(500, "Failed to retrieve real-time metrics");
            }
        }

        /// <summary>
        /// Gets real-time metrics for live dashboard (alternative endpoint)
        /// </summary>
        [HttpGet("metrics/realtime")]
        public async Task<IActionResult> GetRealTimeMetricsAlt()
        {
            try
            {
                var metrics = await _reportingService.GetRealTimeMetricsAsync();
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get real-time metrics");
                return StatusCode(500, "Failed to retrieve real-time metrics");
            }
        }

        /// <summary>
        /// Gets trending data for charts
        /// </summary>
        [HttpGet("trending-data")]
        public async Task<IActionResult> GetTrendingData([FromQuery] Objects.Reporting.TrendPeriod period = Objects.Reporting.TrendPeriod.Last7Days)
        {
            try
            {
                _logger.LogInformation("Getting trending data for period {Period}", period);
                
                // Convert Objects.Reporting.TrendPeriod to Services.Reporting.TrendPeriod
                var servicePeriod = (Services.Reporting.TrendPeriod)(int)period;
                var trends = await _reportingService.GetTrendingDataAsync(servicePeriod);
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get trending data for period {Period}", period);
                return StatusCode(500, "Failed to retrieve trending data");
            }
        }

        /// <summary>
        /// Exports data to CSV format
        /// </summary>
        [HttpPost("export/csv")]
        public async Task<IActionResult> ExportToCsv([FromBody] ReportRequest request)
        {
            try
            {
                _logger.LogInformation("Exporting data to CSV for period {StartDate} to {EndDate}", 
                    request.StartDate, request.EndDate);
                
                var csvData = await _reportingService.ExportToCsvAsync(request);
                var fileName = $"WMSP_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
                
                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export to CSV");
                return StatusCode(500, "Failed to export data to CSV");
            }
        }

        /// <summary>
        /// Exports data to Excel format
        /// </summary>
        [HttpPost("export/excel")]
        public async Task<IActionResult> ExportToExcel([FromBody] ReportRequest request)
        {
            try
            {
                _logger.LogInformation("Exporting data to Excel for period {StartDate} to {EndDate}", 
                    request.StartDate, request.EndDate);
                
                var excelData = await _reportingService.ExportToExcelAsync(request);
                var fileName = $"WMSP_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
                
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export to Excel");
                return StatusCode(500, "Failed to export data to Excel");
            }
        }

        /// <summary>
        /// Exports data to PDF format
        /// </summary>
        [HttpPost("export/pdf")]
        public async Task<IActionResult> ExportToPdf([FromBody] ReportRequest request)
        {
            try
            {
                _logger.LogInformation("Exporting data to PDF for period {StartDate} to {EndDate}", 
                    request.StartDate, request.EndDate);
                
                var pdfData = await _reportingService.ExportToPdfAsync(request);
                var fileName = $"WMSP_Report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
                
                return File(pdfData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export to PDF");
                return StatusCode(500, "Failed to export data to PDF");
            }
        }

        /// <summary>
        /// Gets available report templates
        /// </summary>
        [HttpGet("templates")]
        public async Task<IActionResult> GetReportTemplates()
        {
            try
            {
                var templates = await _reportingService.GetReportTemplatesAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get report templates");
                return StatusCode(500, "Failed to retrieve report templates");
            }
        }

        /// <summary>
        /// Schedules a report to be generated and sent via email
        /// </summary>
        [HttpPost("schedule")]
        public async Task<IActionResult> ScheduleReport([FromBody] Objects.Reporting.ScheduledReportRequest request)
        {
            try
            {
                _logger.LogInformation("Scheduling report {Name} with {Recipients} recipients", 
                    request.Name, request.EmailRecipients.Count);
                
                // Convert to Services.Reporting.ScheduledReportRequest if needed
                var serviceRequest = new Services.Reporting.ScheduledReportRequest
                {
                    Name = request.Name,
                    ReportParameters = request.ReportParameters,
                    CronExpression = request.CronExpression,
                    EmailRecipients = request.EmailRecipients,
                    IsActive = request.IsActive,
                    NextRunDate = request.NextRunDate
                };
                
                var scheduleId = await _reportingService.ScheduleReportAsync(serviceRequest);
                
                return Ok(new { ScheduleId = scheduleId, Message = "Report scheduled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to schedule report {Name}", request.Name);
                return StatusCode(500, "Failed to schedule report");
            }
        }

        /// <summary>
        /// Gets summary statistics for quick dashboard widgets
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummaryStats([FromQuery] int days = 30)
        {
            try
            {
                var endDate = DateTime.UtcNow;
                var startDate = endDate.AddDays(-days);
                
                var request = new ReportRequest
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    ReportType = ReportType.Summary
                };
                
                var analytics = await _reportingService.GenerateAnalyticsAsync(request);
                
                var summary = new
                {
                    TotalBookings = analytics.TotalBookings,
                    ActiveBookings = analytics.ActiveBookings,
                    CancellationRate = analytics.CancellationRate,
                    CompletionRate = analytics.CompletionRate,
                    TopExperience = analytics.ExperienceStats.FirstOrDefault()?.ExperienceName ?? "N/A",
                    BusiestDay = analytics.DayOfWeekStats.OrderByDescending(d => d.BookingCount).FirstOrDefault()?.DayName ?? "N/A",
                    PeakHour = analytics.HourlyStats.OrderByDescending(h => h.BookingCount).FirstOrDefault()?.TimeSlot ?? "N/A"
                };
                
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get summary statistics");
                return StatusCode(500, "Failed to retrieve summary statistics");
            }
        }

        /// <summary>
        /// Gets trending data for charts
        /// </summary>
        [HttpGet("trends/{period}")]
        public async Task<IActionResult> GetTrendingDataByPath(Objects.Reporting.TrendPeriod period)
        {
            try
            {
                _logger.LogInformation("Getting trending data for period {Period}", period);
                
                // Convert Objects.Reporting.TrendPeriod to Services.Reporting.TrendPeriod
                var servicePeriod = (Services.Reporting.TrendPeriod)(int)period;
                var trends = await _reportingService.GetTrendingDataAsync(servicePeriod);
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get trending data for period {Period}", period);
                return StatusCode(500, "Failed to retrieve trending data");
            }
        }
    }
}