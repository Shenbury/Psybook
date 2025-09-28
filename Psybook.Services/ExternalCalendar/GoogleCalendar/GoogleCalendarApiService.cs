using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Psybook.Services.ExternalCalendar.GoogleCalendar
{
    /// <summary>
    /// Google Calendar API integration service
    /// Note: This requires OAuth 2.0 authentication setup
    /// </summary>
    public class GoogleCalendarApiService
    {
        private readonly ILogger<GoogleCalendarApiService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl = "https://www.googleapis.com/calendar/v3";

        public GoogleCalendarApiService(
            ILogger<GoogleCalendarApiService> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Creates an event in Google Calendar (requires OAuth token)
        /// </summary>
        public async Task<GoogleCalendarEventResult> CreateEventAsync(
            string accessToken, 
            string calendarId, 
            GoogleCalendarEvent calendarEvent, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var requestBody = JsonSerializer.Serialize(calendarEvent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(
                    $"{_baseUrl}/calendars/{calendarId}/events", 
                    content, 
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    var createdEvent = JsonSerializer.Deserialize<GoogleCalendarEvent>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    return new GoogleCalendarEventResult
                    {
                        Success = true,
                        Event = createdEvent,
                        EventId = createdEvent?.Id,
                        EventUrl = createdEvent?.HtmlLink
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Google Calendar API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    
                    return new GoogleCalendarEventResult
                    {
                        Success = false,
                        ErrorMessage = $"API error: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Google Calendar event");
                return new GoogleCalendarEventResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Updates an existing event in Google Calendar
        /// </summary>
        public async Task<GoogleCalendarEventResult> UpdateEventAsync(
            string accessToken, 
            string calendarId, 
            string eventId, 
            GoogleCalendarEvent calendarEvent, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var requestBody = JsonSerializer.Serialize(calendarEvent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync(
                    $"{_baseUrl}/calendars/{calendarId}/events/{eventId}", 
                    content, 
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    var updatedEvent = JsonSerializer.Deserialize<GoogleCalendarEvent>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    return new GoogleCalendarEventResult
                    {
                        Success = true,
                        Event = updatedEvent,
                        EventId = updatedEvent?.Id,
                        EventUrl = updatedEvent?.HtmlLink
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Google Calendar API update error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    
                    return new GoogleCalendarEventResult
                    {
                        Success = false,
                        ErrorMessage = $"Update error: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Google Calendar event {EventId}", eventId);
                return new GoogleCalendarEventResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Deletes an event from Google Calendar
        /// </summary>
        public async Task<GoogleCalendarEventResult> DeleteEventAsync(
            string accessToken, 
            string calendarId, 
            string eventId, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.DeleteAsync(
                    $"{_baseUrl}/calendars/{calendarId}/events/{eventId}", 
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return new GoogleCalendarEventResult
                    {
                        Success = true,
                        EventId = eventId
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Google Calendar API delete error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    
                    return new GoogleCalendarEventResult
                    {
                        Success = false,
                        ErrorMessage = $"Delete error: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete Google Calendar event {EventId}", eventId);
                return new GoogleCalendarEventResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }

    /// <summary>
    /// Google Calendar event model
    /// </summary>
    public class GoogleCalendarEvent
    {
        public string? Id { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public GoogleCalendarDateTime Start { get; set; } = new();
        public GoogleCalendarDateTime End { get; set; } = new();
        public List<GoogleCalendarAttendee> Attendees { get; set; } = new();
        public string? HtmlLink { get; set; }
        public string Status { get; set; } = "confirmed";
        public GoogleCalendarReminders? Reminders { get; set; }
    }

    public class GoogleCalendarDateTime
    {
        public string? DateTime { get; set; }
        public string? Date { get; set; }
        public string? TimeZone { get; set; }
    }

    public class GoogleCalendarAttendee
    {
        public string Email { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public bool Optional { get; set; } = false;
        public string ResponseStatus { get; set; } = "needsAction";
    }

    public class GoogleCalendarReminders
    {
        public bool UseDefault { get; set; } = true;
        public List<GoogleCalendarReminderOverride> Overrides { get; set; } = new();
    }

    public class GoogleCalendarReminderOverride
    {
        public string Method { get; set; } = "email";
        public int Minutes { get; set; } = 15;
    }

    /// <summary>
    /// Result of Google Calendar API operations
    /// </summary>
    public class GoogleCalendarEventResult
    {
        public bool Success { get; set; }
        public GoogleCalendarEvent? Event { get; set; }
        public string? EventId { get; set; }
        public string? EventUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }
}