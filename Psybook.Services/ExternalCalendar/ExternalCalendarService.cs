using System.Globalization;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using Psybook.Objects.DbModels;

namespace Psybook.Services.ExternalCalendar
{
    /// <summary>
    /// Implementation of external calendar integration services
    /// </summary>
    public class ExternalCalendarService : IExternalCalendarService
    {
        private readonly ILogger<ExternalCalendarService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public ExternalCalendarService(ILogger<ExternalCalendarService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public IEnumerable<CalendarProvider> SupportedProviders => new[]
        {
            CalendarProvider.GoogleCalendar,
            CalendarProvider.OutlookCalendar,
            CalendarProvider.AppleCalendar,
            CalendarProvider.ICalendar
        };

        public async Task<CalendarIntegrationResult> CreateEventAsync(CalendarProvider provider, ExternalCalendarEvent calendarEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Creating calendar event for provider {Provider}: {Title}", provider, calendarEvent.Title);

                return provider switch
                {
                    CalendarProvider.GoogleCalendar => await CreateGoogleCalendarEventAsync(calendarEvent, cancellationToken),
                    CalendarProvider.OutlookCalendar => await CreateOutlookCalendarEventAsync(calendarEvent, cancellationToken),
                    CalendarProvider.AppleCalendar => await CreateAppleCalendarEventAsync(calendarEvent, cancellationToken),
                    CalendarProvider.ICalendar => await CreateICalendarEventAsync(calendarEvent, cancellationToken),
                    _ => CalendarIntegrationResult.CreateError(provider, "Unsupported calendar provider")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create calendar event for provider {Provider}", provider);
                return CalendarIntegrationResult.CreateError(provider, ex.Message);
            }
        }

        public async Task<CalendarIntegrationResult> UpdateEventAsync(CalendarProvider provider, string eventId, ExternalCalendarEvent calendarEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating calendar event {EventId} for provider {Provider}", eventId, provider);

                // For URL-based providers, we typically can't update events
                // This would require API integration with OAuth for Google/Outlook
                return provider switch
                {
                    CalendarProvider.GoogleCalendar => CalendarIntegrationResult.CreateError(provider, "Update requires Google Calendar API integration"),
                    CalendarProvider.OutlookCalendar => CalendarIntegrationResult.CreateError(provider, "Update requires Microsoft Graph API integration"),
                    CalendarProvider.AppleCalendar => CalendarIntegrationResult.CreateError(provider, "Apple Calendar does not support programmatic updates"),
                    CalendarProvider.ICalendar => await CreateICalendarEventAsync(calendarEvent, cancellationToken), // Generate new iCal file
                    _ => CalendarIntegrationResult.CreateError(provider, "Unsupported calendar provider")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update calendar event {EventId} for provider {Provider}", eventId, provider);
                return CalendarIntegrationResult.CreateError(provider, ex.Message);
            }
        }

        public async Task<CalendarIntegrationResult> DeleteEventAsync(CalendarProvider provider, string eventId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Deleting calendar event {EventId} for provider {Provider}", eventId, provider);

                // For URL-based providers, we typically can't delete events
                // This would require API integration with OAuth
                return provider switch
                {
                    CalendarProvider.GoogleCalendar => CalendarIntegrationResult.CreateError(provider, "Delete requires Google Calendar API integration"),
                    CalendarProvider.OutlookCalendar => CalendarIntegrationResult.CreateError(provider, "Delete requires Microsoft Graph API integration"),
                    CalendarProvider.AppleCalendar => CalendarIntegrationResult.CreateError(provider, "Apple Calendar does not support programmatic deletion"),
                    CalendarProvider.ICalendar => CalendarIntegrationResult.CreateSuccess(provider, null, "iCalendar file can be removed manually"),
                    _ => CalendarIntegrationResult.CreateError(provider, "Unsupported calendar provider")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete calendar event {EventId} for provider {Provider}", eventId, provider);
                return CalendarIntegrationResult.CreateError(provider, ex.Message);
            }
        }

        public async Task<byte[]> GenerateICalendarFileAsync(ExternalCalendarEvent calendarEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Generating iCalendar file for event: {Title}", calendarEvent.Title);

                var icalContent = GenerateICalendarContent(calendarEvent);
                return Encoding.UTF8.GetBytes(icalContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate iCalendar file for event: {Title}", calendarEvent.Title);
                throw;
            }
        }

        public Dictionary<CalendarProvider, string> GenerateCalendarUrls(ExternalCalendarEvent calendarEvent)
        {
            var urls = new Dictionary<CalendarProvider, string>();

            try
            {
                // Google Calendar
                urls[CalendarProvider.GoogleCalendar] = GenerateGoogleCalendarUrl(calendarEvent);

                // Outlook Calendar
                urls[CalendarProvider.OutlookCalendar] = GenerateOutlookCalendarUrl(calendarEvent);

                // Apple Calendar (uses webcal:// protocol)
                urls[CalendarProvider.AppleCalendar] = GenerateAppleCalendarUrl(calendarEvent);

                _logger.LogInformation("Generated calendar URLs for event: {Title}", calendarEvent.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate calendar URLs for event: {Title}", calendarEvent.Title);
            }

            return urls;
        }

        public async Task<List<CalendarIntegrationResult>> SyncBookingAsync(CalendarSlot booking, CalendarIntegrationOptions options, CancellationToken cancellationToken = default)
        {
            var results = new List<CalendarIntegrationResult>();
            var calendarEvent = ExternalCalendarEvent.FromCalendarSlot(booking);

            foreach (var provider in options.EnabledProviders)
            {
                try
                {
                    var result = await CreateEventAsync(provider, calendarEvent, cancellationToken);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to sync booking {BookingId} with {Provider}", booking.Id, provider);
                    results.Add(CalendarIntegrationResult.CreateError(provider, ex.Message));
                }
            }

            return results;
        }

        #region Private Methods

        private async Task<CalendarIntegrationResult> CreateGoogleCalendarEventAsync(ExternalCalendarEvent calendarEvent, CancellationToken cancellationToken)
        {
            // For now, we'll return the URL. Full API integration would require OAuth setup
            var url = GenerateGoogleCalendarUrl(calendarEvent);
            return CalendarIntegrationResult.CreateSuccess(CalendarProvider.GoogleCalendar, null, url);
        }

        private async Task<CalendarIntegrationResult> CreateOutlookCalendarEventAsync(ExternalCalendarEvent calendarEvent, CancellationToken cancellationToken)
        {
            // For now, we'll return the URL. Full API integration would require Microsoft Graph API
            var url = GenerateOutlookCalendarUrl(calendarEvent);
            return CalendarIntegrationResult.CreateSuccess(CalendarProvider.OutlookCalendar, null, url);
        }

        private async Task<CalendarIntegrationResult> CreateAppleCalendarEventAsync(ExternalCalendarEvent calendarEvent, CancellationToken cancellationToken)
        {
            // Apple Calendar typically uses iCalendar files
            var url = GenerateAppleCalendarUrl(calendarEvent);
            return CalendarIntegrationResult.CreateSuccess(CalendarProvider.AppleCalendar, null, url);
        }

        private async Task<CalendarIntegrationResult> CreateICalendarEventAsync(ExternalCalendarEvent calendarEvent, CancellationToken cancellationToken)
        {
            // Generate iCalendar content
            var icalData = await GenerateICalendarFileAsync(calendarEvent, cancellationToken);
            return CalendarIntegrationResult.CreateSuccess(CalendarProvider.ICalendar, null, "iCalendar file generated");
        }

        private string GenerateGoogleCalendarUrl(ExternalCalendarEvent calendarEvent)
        {
            var startTime = calendarEvent.StartTime.ToString("yyyyMMddTHHmmssZ");
            var endTime = calendarEvent.EndTime?.ToString("yyyyMMddTHHmmssZ") ?? 
                         calendarEvent.StartTime.AddHours(2).ToString("yyyyMMddTHHmmssZ");

            var parameters = new Dictionary<string, string>
            {
                ["action"] = "TEMPLATE",
                ["text"] = calendarEvent.Title,
                ["dates"] = $"{startTime}/{endTime}",
                ["details"] = calendarEvent.Description,
                ["location"] = calendarEvent.Location
            };

            var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));
            return $"https://calendar.google.com/calendar/render?{queryString}";
        }

        private string GenerateOutlookCalendarUrl(ExternalCalendarEvent calendarEvent)
        {
            var startTime = calendarEvent.StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var endTime = calendarEvent.EndTime?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? 
                         calendarEvent.StartTime.AddHours(2).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            var parameters = new Dictionary<string, string>
            {
                ["subject"] = calendarEvent.Title,
                ["startdt"] = startTime,
                ["enddt"] = endTime,
                ["body"] = calendarEvent.Description,
                ["location"] = calendarEvent.Location
            };

            var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));
            return $"https://outlook.live.com/calendar/0/deeplink/compose?{queryString}";
        }

        private string GenerateAppleCalendarUrl(ExternalCalendarEvent calendarEvent)
        {
            // Apple Calendar works best with iCalendar data URLs
            var icalContent = GenerateICalendarContent(calendarEvent);
            var encodedContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(icalContent));
            return $"data:text/calendar;base64,{encodedContent}";
        }

        private string GenerateICalendarContent(ExternalCalendarEvent calendarEvent)
        {
            var sb = new StringBuilder();
            var now = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            var uid = Guid.NewGuid().ToString();

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//West Midlands Safari Park//VIP Booking System//EN");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");
            
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:{uid}");
            sb.AppendLine($"DTSTAMP:{now}");
            
            if (calendarEvent.AllDay)
            {
                sb.AppendLine($"DTSTART;VALUE=DATE:{calendarEvent.StartTime:yyyyMMdd}");
                if (calendarEvent.EndTime.HasValue)
                {
                    sb.AppendLine($"DTEND;VALUE=DATE:{calendarEvent.EndTime.Value:yyyyMMdd}");
                }
            }
            else
            {
                sb.AppendLine($"DTSTART:{calendarEvent.StartTime:yyyyMMddTHHmmssZ}");
                if (calendarEvent.EndTime.HasValue)
                {
                    sb.AppendLine($"DTEND:{calendarEvent.EndTime.Value:yyyyMMddTHHmmssZ}");
                }
            }
            
            sb.AppendLine($"SUMMARY:{EscapeCalendarText(calendarEvent.Title)}");
            sb.AppendLine($"DESCRIPTION:{EscapeCalendarText(calendarEvent.Description)}");
            sb.AppendLine($"LOCATION:{EscapeCalendarText(calendarEvent.Location)}");
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("TRANSP:OPAQUE");
            
            // Add custom properties
            foreach (var prop in calendarEvent.CustomProperties)
            {
                sb.AppendLine($"X-{prop.Key.ToUpper()}:{EscapeCalendarText(prop.Value)}");
            }
            
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            return sb.ToString();
        }

        private string EscapeCalendarText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text
                .Replace("\\", "\\\\")
                .Replace("\n", "\\n")
                .Replace("\r", "")
                .Replace(",", "\\,")
                .Replace(";", "\\;");
        }

        #endregion
    }
}