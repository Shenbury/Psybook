using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Psybook.Objects.DbModels;
using Psybook.Services.API.BookingService;
using Psybook.Services.ExternalCalendar;

namespace Psybook.API.Controllers
{
    /// <summary>
    /// API controller for external calendar integration operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarIntegrationController : ControllerBase
    {
        private readonly ILogger<CalendarIntegrationController> _logger;
        private readonly IExternalCalendarService _calendarService;
        private readonly IBookingService _bookingService;

        public CalendarIntegrationController(
            ILogger<CalendarIntegrationController> logger,
            IExternalCalendarService calendarService,
            IBookingService bookingService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _calendarService = calendarService ?? throw new ArgumentNullException(nameof(calendarService));
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        }

        /// <summary>
        /// Generates an iCalendar (.ics) file for a specific booking
        /// </summary>
        [HttpGet("booking/{bookingId}/icalendar")]
        public async Task<IActionResult> GetICalendarFile(Guid bookingId)
        {
            try
            {
                _logger.LogInformation("Generating iCalendar file for booking {BookingId}", bookingId);

                var booking = await _bookingService.GetCalendarSlotByIdAsync(bookingId);
                if (booking == null)
                {
                    return NotFound($"Booking with ID {bookingId} not found");
                }

                var calendarEvent = ExternalCalendarEvent.FromCalendarSlot(booking);
                var icalData = await _calendarService.GenerateICalendarFileAsync(calendarEvent);

                var fileName = $"VIP_Experience_{booking.Start:yyyyMMdd}_{booking.FirstName}_{booking.LastName}.ics";
                
                return File(icalData, "text/calendar", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate iCalendar file for booking {BookingId}", bookingId);
                return StatusCode(500, "Failed to generate calendar file");
            }
        }

        /// <summary>
        /// Gets calendar URLs for a specific booking
        /// </summary>
        [HttpGet("booking/{bookingId}/urls")]
        public async Task<IActionResult> GetCalendarUrls(Guid bookingId)
        {
            try
            {
                _logger.LogInformation("Generating calendar URLs for booking {BookingId}", bookingId);

                var booking = await _bookingService.GetCalendarSlotByIdAsync(bookingId);
                if (booking == null)
                {
                    return NotFound($"Booking with ID {bookingId} not found");
                }

                var calendarEvent = ExternalCalendarEvent.FromCalendarSlot(booking);
                var urls = _calendarService.GenerateCalendarUrls(calendarEvent);

                return Ok(new CalendarUrlsResponse
                {
                    BookingId = bookingId,
                    Urls = urls.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value
                    )
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate calendar URLs for booking {BookingId}", bookingId);
                return StatusCode(500, "Failed to generate calendar URLs");
            }
        }

        /// <summary>
        /// Syncs a booking with external calendars
        /// </summary>
        [HttpPost("booking/{bookingId}/sync")]
        public async Task<IActionResult> SyncBooking(Guid bookingId, [FromBody] CalendarSyncRequest request)
        {
            try
            {
                _logger.LogInformation("Syncing booking {BookingId} with external calendars", bookingId);

                var booking = await _bookingService.GetCalendarSlotByIdAsync(bookingId);
                if (booking == null)
                {
                    return NotFound($"Booking with ID {bookingId} not found");
                }

                var options = new CalendarIntegrationOptions
                {
                    AutoSync = request.AutoSync,
                    EnabledProviders = request.Providers,
                    IncludeCustomerDetails = request.IncludeCustomerDetails,
                    SendInvitations = request.SendInvitations
                };

                var results = await _calendarService.SyncBookingAsync(booking, options);

                return Ok(new CalendarSyncResponse
                {
                    BookingId = bookingId,
                    SyncResults = results.Select(r => new SyncResultDto
                    {
                        Provider = r.Provider.ToString(),
                        Success = r.Success,
                        EventId = r.EventId,
                        EventUrl = r.EventUrl,
                        ErrorMessage = r.ErrorMessage
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync booking {BookingId} with external calendars", bookingId);
                return StatusCode(500, "Failed to sync with external calendars");
            }
        }

        /// <summary>
        /// Webhook endpoint for calendar provider notifications
        /// </summary>
        [HttpPost("webhook/{provider}")]
        public async Task<IActionResult> HandleWebhook(string provider, [FromBody] object webhookData)
        {
            try
            {
                _logger.LogInformation("Received webhook from {Provider}", provider);

                // Handle webhook based on provider
                switch (provider.ToLowerInvariant())
                {
                    case "google":
                        await HandleGoogleWebhook(webhookData);
                        break;
                    case "outlook":
                        await HandleOutlookWebhook(webhookData);
                        break;
                    default:
                        _logger.LogWarning("Unknown webhook provider: {Provider}", provider);
                        return BadRequest($"Unknown provider: {provider}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle webhook from {Provider}", provider);
                return StatusCode(500, "Failed to process webhook");
            }
        }

        /// <summary>
        /// Gets supported calendar providers
        /// </summary>
        [HttpGet("providers")]
        public IActionResult GetSupportedProviders()
        {
            var providers = _calendarService.SupportedProviders.Select(p => new
            {
                Name = p.ToString(),
                DisplayName = GetProviderDisplayName(p),
                SupportsApi = GetProviderApiSupport(p)
            });

            return Ok(providers);
        }

        #region Private Methods

        private async Task HandleGoogleWebhook(object webhookData)
        {
            // Implement Google Calendar webhook handling
            _logger.LogInformation("Processing Google Calendar webhook");
            // TODO: Implement based on Google Calendar API webhook specification
        }

        private async Task HandleOutlookWebhook(object webhookData)
        {
            // Implement Outlook Calendar webhook handling
            _logger.LogInformation("Processing Outlook Calendar webhook");
            // TODO: Implement based on Microsoft Graph API webhook specification
        }

        private static string GetProviderDisplayName(CalendarProvider provider)
        {
            return provider switch
            {
                CalendarProvider.GoogleCalendar => "Google Calendar",
                CalendarProvider.OutlookCalendar => "Outlook Calendar",
                CalendarProvider.AppleCalendar => "Apple Calendar",
                CalendarProvider.ICalendar => "iCalendar (.ics)",
                CalendarProvider.CalDav => "CalDAV",
                _ => provider.ToString()
            };
        }

        private static bool GetProviderApiSupport(CalendarProvider provider)
        {
            return provider switch
            {
                CalendarProvider.GoogleCalendar => true,
                CalendarProvider.OutlookCalendar => true,
                CalendarProvider.AppleCalendar => false,
                CalendarProvider.ICalendar => false,
                CalendarProvider.CalDav => true,
                _ => false
            };
        }

        #endregion
    }

    #region DTOs

    public class CalendarUrlsResponse
    {
        public Guid BookingId { get; set; }
        public Dictionary<string, string> Urls { get; set; } = new();
    }

    public class CalendarSyncRequest
    {
        public bool AutoSync { get; set; }
        public List<CalendarProvider> Providers { get; set; } = new();
        public bool IncludeCustomerDetails { get; set; } = true;
        public bool SendInvitations { get; set; } = false;
    }

    public class CalendarSyncResponse
    {
        public Guid BookingId { get; set; }
        public List<SyncResultDto> SyncResults { get; set; } = new();
    }

    public class SyncResultDto
    {
        public string Provider { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? EventId { get; set; }
        public string? EventUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }

    #endregion
}