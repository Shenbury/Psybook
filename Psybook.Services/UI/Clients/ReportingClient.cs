using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Psybook.Objects.Reporting;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Psybook.Services.UI.Clients
{
    /// <summary>
    /// HTTP client for reporting-related API operations.
    /// Handles communication with the reporting API for analytics and dashboard data.
    /// </summary>
    public sealed class ReportingClient : IReportingClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReportingClient> _logger;
        private readonly BookingClientOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingClient"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Factory for creating HTTP clients.</param>
        /// <param name="logger">Logger for tracking operations and errors.</param>
        /// <param name="options">Configuration options for the client.</param>
        public ReportingClient(
            IHttpClientFactory httpClientFactory, 
            ILogger<ReportingClient> logger,
            IOptions<BookingClientOptions>? options = null)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? new BookingClientOptions();
        }

        /// <inheritdoc />
        public async Task<DashboardSummary> GetDashboardSummaryAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await GetDashboardSummaryInternalAsync(startDate, endDate, cancellationToken),
                _logger,
                "retrieving dashboard summary from API",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<BookingAnalytics> GetAnalyticsAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await GetAnalyticsInternalAsync(request, cancellationToken),
                _logger,
                $"retrieving analytics for {request.ReportType} from API",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TrendingData> GetTrendingDataAsync(TrendPeriod period = TrendPeriod.Last30Days, CancellationToken cancellationToken = default)
        {
            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await GetTrendingDataInternalAsync(period, cancellationToken),
                _logger,
                $"retrieving trending data for {period} from API",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ReportExport> GenerateReportAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await GenerateReportInternalAsync(request, cancellationToken),
                _logger,
                $"generating {request.Format} report for {request.ReportType}",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<RealTimeMetrics> GetRealTimeMetricsAsync(CancellationToken cancellationToken = default)
        {
            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await GetRealTimeMetricsInternalAsync(cancellationToken),
                _logger,
                "retrieving real-time metrics from API",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<ReportTemplate>> GetReportTemplatesAsync(CancellationToken cancellationToken = default)
        {
            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await GetReportTemplatesInternalAsync(cancellationToken),
                _logger,
                "retrieving report templates from API",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Guid> ScheduleReportAsync(ScheduledReportRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await ScheduleReportInternalAsync(request, cancellationToken),
                _logger,
                $"scheduling report {request.Name}",
                cancellationToken);
        }

        /// <summary>
        /// Internal implementation for retrieving dashboard summary.
        /// </summary>
        private async Task<DashboardSummary> GetDashboardSummaryInternalAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            
            var queryBuilder = new StringBuilder();
            if (startDate.HasValue)
            {
                queryBuilder.Append($"?startDate={startDate.Value:yyyy-MM-ddTHH:mm:ss.fffZ}");
            }
            if (endDate.HasValue)
            {
                var separator = queryBuilder.Length > 0 ? "&" : "?";
                queryBuilder.Append($"{separator}endDate={endDate.Value:yyyy-MM-ddTHH:mm:ss.fffZ}");
            }

            var url = ReportingClientConstants.ApiRoutes.GetDashboardSummary + queryBuilder.ToString();
            var dashboardSummary = await httpClient.GetFromJsonAsync<DashboardSummary>(url, cancellationToken);

            if (_options.ValidateResponses)
            {
                HttpClientExtensions.ValidateResponse(dashboardSummary, _logger, "GetDashboardSummary");
            }

            return dashboardSummary ?? new DashboardSummary();
        }

        /// <summary>
        /// Internal implementation for retrieving analytics.
        /// </summary>
        private async Task<BookingAnalytics> GetAnalyticsInternalAsync(ReportRequest request, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            
            var response = await httpClient.PostAsJsonAsync(
                ReportingClientConstants.ApiRoutes.GetAnalytics, 
                request, 
                cancellationToken);

            response.EnsureSuccessStatusCode();
            var analytics = await response.Content.ReadFromJsonAsync<BookingAnalytics>(cancellationToken: cancellationToken);

            if (_options.ValidateResponses)
            {
                HttpClientExtensions.ValidateResponse(analytics, _logger, "GetAnalytics");
            }

            return analytics ?? new BookingAnalytics();
        }

        /// <summary>
        /// Internal implementation for retrieving trending data.
        /// </summary>
        private async Task<TrendingData> GetTrendingDataInternalAsync(TrendPeriod period, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            
            var url = $"{ReportingClientConstants.ApiRoutes.GetTrendingData}?period={period}";
            var trendingData = await httpClient.GetFromJsonAsync<TrendingData>(url, cancellationToken);

            if (_options.ValidateResponses)
            {
                HttpClientExtensions.ValidateResponse(trendingData, _logger, "GetTrendingData");
            }

            return trendingData ?? new TrendingData();
        }

        /// <summary>
        /// Internal implementation for generating a report.
        /// </summary>
        private async Task<ReportExport> GenerateReportInternalAsync(ReportRequest request, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            
            var response = await httpClient.PostAsJsonAsync(
                ReportingClientConstants.ApiRoutes.GenerateReport, 
                request, 
                cancellationToken);

            response.EnsureSuccessStatusCode();

            // For file downloads, we need to handle the response differently
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileName = ExtractFileNameFromResponse(response) ?? GenerateDefaultFileName(request);
            var data = await response.Content.ReadAsByteArrayAsync(cancellationToken);

            return new ReportExport
            {
                FileName = fileName,
                Format = request.Format,
                Data = data,
                ContentType = contentType,
                FileSize = data.Length,
                Parameters = request
            };
        }

        /// <summary>
        /// Internal implementation for retrieving real-time metrics.
        /// </summary>
        private async Task<RealTimeMetrics> GetRealTimeMetricsInternalAsync(CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            
            var metrics = await httpClient.GetFromJsonAsync<RealTimeMetrics>(
                ReportingClientConstants.ApiRoutes.GetRealTimeMetrics, 
                cancellationToken);

            if (_options.ValidateResponses)
            {
                HttpClientExtensions.ValidateResponse(metrics, _logger, "GetRealTimeMetrics");
            }

            return metrics ?? new RealTimeMetrics();
        }

        /// <summary>
        /// Internal implementation for retrieving report templates.
        /// </summary>
        private async Task<List<ReportTemplate>> GetReportTemplatesInternalAsync(CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            
            var templates = await httpClient.GetFromJsonAsync<List<ReportTemplate>>(
                ReportingClientConstants.ApiRoutes.GetReportTemplates, 
                cancellationToken);

            if (_options.ValidateResponses)
            {
                HttpClientExtensions.ValidateResponse(templates, _logger, "GetReportTemplates");
            }

            return templates ?? new List<ReportTemplate>();
        }

        /// <summary>
        /// Internal implementation for scheduling a report.
        /// </summary>
        private async Task<Guid> ScheduleReportInternalAsync(ScheduledReportRequest request, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            
            var response = await httpClient.PostAsJsonAsync(
                ReportingClientConstants.ApiRoutes.ScheduleReport, 
                request, 
                cancellationToken);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            using var jsonDoc = JsonDocument.Parse(responseContent);
            if (jsonDoc.RootElement.TryGetProperty("ScheduleId", out var scheduleIdProperty))
            {
                if (Guid.TryParse(scheduleIdProperty.GetString(), out var scheduleId))
                {
                    return scheduleId;
                }
            }

            throw new InvalidOperationException("Failed to extract schedule ID from API response");
        }

        /// <summary>
        /// Creates and configures an HTTP client for reporting API operations.
        /// </summary>
        /// <returns>A configured HTTP client.</returns>
        private HttpClient CreateConfiguredHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient(ReportingClientConstants.HttpClientName);
            httpClient.Timeout = _options.RequestTimeout;
            return httpClient;
        }

        /// <summary>
        /// Extracts the filename from HTTP response headers.
        /// </summary>
        private static string? ExtractFileNameFromResponse(HttpResponseMessage response)
        {
            var contentDisposition = response.Content.Headers.ContentDisposition;
            return contentDisposition?.FileName?.Trim('"');
        }

        /// <summary>
        /// Generates a default filename for the report.
        /// </summary>
        private static string GenerateDefaultFileName(ReportRequest request)
        {
            var extension = request.Format switch
            {
                ReportFormat.PDF => ".pdf",
                ReportFormat.Excel => ".xlsx",
                ReportFormat.CSV => ".csv",
                ReportFormat.JSON => ".json",
                _ => ".txt"
            };

            return $"WMSP_Report_{DateTime.UtcNow:yyyyMMdd_HHmmss}{extension}";
        }
    }
}