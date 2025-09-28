using Psybook.Objects.DbModels;

namespace Psybook.Services.ExternalCalendar
{
    /// <summary>
    /// Represents an external calendar event that can be created or synced
    /// </summary>
    public class ExternalCalendarEvent
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public required string Location { get; set; }
        public bool AllDay { get; set; }
        public string? TimeZone { get; set; } = "UTC";
        public List<string> Attendees { get; set; } = new();
        public Dictionary<string, string> CustomProperties { get; set; } = new();
        
        /// <summary>
        /// Creates an ExternalCalendarEvent from a CalendarSlot booking
        /// </summary>
        public static ExternalCalendarEvent FromCalendarSlot(CalendarSlot booking)
        {
            var eventTitle = $"{booking.Title} - {booking.FirstName} {booking.LastName}";
            var description = CreateDescription(booking);
            
            return new ExternalCalendarEvent
            {
                Title = eventTitle,
                Description = description,
                StartTime = booking.Start,
                EndTime = booking.End,
                Location = booking.Location,
                AllDay = booking.AllDay,
                Attendees = new List<string> { $"{booking.FirstName} {booking.LastName}" },
                CustomProperties = new Dictionary<string, string>
                {
                    ["BookingId"] = booking.Id.ToString(),
                    ["Status"] = booking.Status.ToString(),
                    ["Experience"] = booking.BookingExperience.ToString(),
                    ["ContactNumber"] = booking.ContactNumber ?? "",
                    ["Address"] = $"{booking.FirstLineAddress}, {booking.Postcode}".Trim(", ".ToCharArray())
                }
            };
        }
        
        private static string CreateDescription(CalendarSlot booking)
        {
            var description = $@"VIP Experience Booking Details:

Experience: {booking.Title}
Status: {booking.StatusDisplayName}
Date: {booking.Start:dddd, MMMM dd, yyyy}
Time: {(booking.AllDay ? "All Day" : $"{booking.Start:HH:mm}" + (booking.End.HasValue ? $" - {booking.End.Value:HH:mm}" : ""))}
Location: {booking.Location}

Customer Information:
Name: {booking.FirstName} {booking.LastName}
Contact: {booking.ContactNumber}
Address: {booking.FirstLineAddress}, {booking.Postcode}

Experience Type: {booking.BookingExperience}
Booking ID: {booking.Id}

{(string.IsNullOrEmpty(booking.Notes) ? "" : $"Notes: {booking.Notes}")}

---
West Midlands Safari Park VIP Experience
";
            return description;
        }
    }
    
    /// <summary>
    /// Response from calendar integration operations
    /// </summary>
    public class CalendarIntegrationResult
    {
        public bool Success { get; set; }
        public string? EventId { get; set; }
        public string? EventUrl { get; set; }
        public string? ErrorMessage { get; set; }
        public CalendarProvider Provider { get; set; }
        
        public static CalendarIntegrationResult CreateSuccess(CalendarProvider provider, string? eventId = null, string? eventUrl = null)
        {
            return new CalendarIntegrationResult
            {
                Success = true,
                Provider = provider,
                EventId = eventId,
                EventUrl = eventUrl
            };
        }
        
        public static CalendarIntegrationResult CreateError(CalendarProvider provider, string errorMessage)
        {
            return new CalendarIntegrationResult
            {
                Success = false,
                Provider = provider,
                ErrorMessage = errorMessage
            };
        }
    }
    
    /// <summary>
    /// Supported external calendar providers
    /// </summary>
    public enum CalendarProvider
    {
        GoogleCalendar,
        OutlookCalendar,
        AppleCalendar,
        ICalendar,
        CalDav
    }
    
    /// <summary>
    /// Calendar integration options
    /// </summary>
    public class CalendarIntegrationOptions
    {
        public bool AutoSync { get; set; } = false;
        public List<CalendarProvider> EnabledProviders { get; set; } = new();
        public string? DefaultTimeZone { get; set; } = "UTC";
        public bool IncludeCustomerDetails { get; set; } = true;
        public bool SendInvitations { get; set; } = false;
    }
}