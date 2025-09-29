namespace Psybook.Services.UI.Clients
{
    /// <summary>
    /// Constants for the ReportingClient
    /// </summary>
    public static class ReportingClientConstants
    {
        /// <summary>
        /// The name of the HTTP client used for reporting API operations
        /// </summary>
        public const string HttpClientName = "psybook-api";

        /// <summary>
        /// API route constants for reporting endpoints
        /// </summary>
        public static class ApiRoutes
        {
            public const string GetDashboardSummary = "api/Reporting/dashboard";
            public const string GetAnalytics = "api/Reporting/analytics";
            public const string GetTrendingData = "api/Reporting/trending-data";
            public const string GenerateReport = "api/Reporting/generate";
            public const string GetRealTimeMetrics = "api/Reporting/realtime-metrics";
            public const string GetReportTemplates = "api/Reporting/templates";
            public const string ScheduleReport = "api/Reporting/schedule";
        }
    }
}