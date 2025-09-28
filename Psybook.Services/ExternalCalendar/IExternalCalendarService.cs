using Psybook.Objects.DbModels;

namespace Psybook.Services.ExternalCalendar
{
    /// <summary>
    /// Interface for external calendar integration services
    /// </summary>
    public interface IExternalCalendarService
    {
        /// <summary>
        /// Gets the supported calendar providers
        /// </summary>
        IEnumerable<CalendarProvider> SupportedProviders { get; }
        
        /// <summary>
        /// Creates a calendar event in the specified external calendar
        /// </summary>
        Task<CalendarIntegrationResult> CreateEventAsync(CalendarProvider provider, ExternalCalendarEvent calendarEvent, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Updates an existing calendar event
        /// </summary>
        Task<CalendarIntegrationResult> UpdateEventAsync(CalendarProvider provider, string eventId, ExternalCalendarEvent calendarEvent, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Deletes a calendar event
        /// </summary>
        Task<CalendarIntegrationResult> DeleteEventAsync(CalendarProvider provider, string eventId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Generates an iCalendar (.ics) file for download
        /// </summary>
        Task<byte[]> GenerateICalendarFileAsync(ExternalCalendarEvent calendarEvent, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Generates calendar URLs for quick add to various providers
        /// </summary>
        Dictionary<CalendarProvider, string> GenerateCalendarUrls(ExternalCalendarEvent calendarEvent);
        
        /// <summary>
        /// Syncs a booking with external calendars based on user preferences
        /// </summary>
        Task<List<CalendarIntegrationResult>> SyncBookingAsync(CalendarSlot booking, CalendarIntegrationOptions options, CancellationToken cancellationToken = default);
    }
}